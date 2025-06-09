using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;

namespace ShopTex.Infrastructure.OrdersProduct;

public class OrderProductEntityTypeConfiguration 
    : IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> builder)
    {
        builder.HasKey(op => new { op.OrderId, op.ProductId });

        builder.Property(op => op.OrderId)
            .HasConversion(id => id.AsGuid(), guid => new OrderId(guid))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(op => op.ProductId).IsRequired();
        builder.Property(op => op.Amount).IsRequired();
        builder.Property(op => op.Price).IsRequired();

        builder.HasOne(op => op.Order)
            .WithMany(o => o.Products)
            .HasForeignKey(op => op.OrderId)
            .HasPrincipalKey(o => o.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}