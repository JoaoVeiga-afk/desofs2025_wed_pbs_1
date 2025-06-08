using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Users;
using ShopTex.Infrastructure.Users;
using ShopTex.Domain.Stores;
using ShopTex.Infrastructure.Stores;
using ShopTex.Domain.Products;
using ShopTex.Infrastructure.Products;

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
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Store> Store { get; set; } = null!;
    public DbSet<Product> Product { get; set; } = null!;

}