using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Tests;

public class TodoDbTests
{
    private static AppDbContext NewDb() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    [Fact]
    public async Task Can_add_and_read_a_todo()
    {
        await using var db = NewDb();
        db.Todos.Add(new TodoItem { Title = "Write code" });
        await db.SaveChangesAsync();

        var item = await db.Todos.SingleAsync();
        Assert.Equal("Write code", item.Title);
        Assert.False(item.IsDone);
    }

    [Fact]
    public async Task Can_mark_todo_done()
    {
        await using var db = NewDb();
        var todo = new TodoItem { Title = "Ship it" };
        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        todo.IsDone = true;
        await db.SaveChangesAsync();

        Assert.True((await db.Todos.SingleAsync()).IsDone);
    }
}
