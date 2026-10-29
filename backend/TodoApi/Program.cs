using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Create the schema on startup, retrying while the DB container boots.
// For production use EF Core migrations instead of EnsureCreated.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    for (var attempt = 1; ; attempt++)
    {
        try { db.Database.EnsureCreated(); break; }
        catch when (attempt < 10)
        {
            app.Logger.LogWarning("Database not ready (attempt {Attempt}), retrying...", attempt);
            Thread.Sleep(2000);
        }
    }
}

app.MapGet("/health", () => Results.Ok("healthy"));

var todos = app.MapGroup("/api/todos");

todos.MapGet("/", async (AppDbContext db) =>
    await db.Todos.OrderBy(t => t.Id).ToListAsync());

todos.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Todos.FindAsync(id) is { } todo ? Results.Ok(todo) : Results.NotFound());

todos.MapPost("/", async (CreateTodo dto, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { error = "Title is required" });

    var todo = new TodoItem { Title = dto.Title.Trim() };
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/api/todos/{todo.Id}", todo);
});

todos.MapPut("/{id:int}", async (int id, UpdateTodo dto, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(dto.Title))
        return Results.BadRequest(new { error = "Title is required" });

    todo.Title = dto.Title.Trim();
    todo.IsDone = dto.IsDone;
    await db.SaveChangesAsync();
    return Results.Ok(todo);
});

todos.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

record CreateTodo(string Title);
record UpdateTodo(string Title, bool IsDone);
