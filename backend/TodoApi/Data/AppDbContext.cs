using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<TodoItem>(e =>
        {
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
        });
    }
}
