using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Models;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.TodoItems)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TodoItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.TodoItems)
            .HasForeignKey(t => t.UserId);
    }
}
