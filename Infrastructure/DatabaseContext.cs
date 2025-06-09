using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Users;
using ShopTex.Infrastructure.Users;
using ShopTex.Domain.Stores;
using ShopTex.Infrastructure.Stores;

namespace ShopTex.Models;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        this.Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StoreEntityTypeConfiguration());
    }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Store> Store { get; set; } = null!;
}