using Microsoft.EntityFrameworkCore;
using UserManager.Domain.Users;
using UserManager.Infrastructure.Users;

namespace UserManager.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        this.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
    }

    public DbSet<User> Users { get; set; } = null!;
}