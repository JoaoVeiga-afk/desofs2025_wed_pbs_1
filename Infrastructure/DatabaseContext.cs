using Microsoft.EntityFrameworkCore;
using ShopTex.Domain.Users;
using ShopTex.Infrastructure.Users;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Infrastructure.Orders;
using ShopTex.Infrastructure.OrdersProduct;
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
        modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new OrderProductEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new StoreEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
    }

    public DbSet<User> User { get; set; } = null!;
    public DbSet<Order> Order { get; set; } = null!;
    public DbSet<OrderProduct> OrderProduct { get; set; } = null!;
    public DbSet<Store> Store { get; set; } = null!;
    public DbSet<Product> Product { get; set; } = null!;

}