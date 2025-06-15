using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Orders;
using ShopTex.Domain.OrdersProduct;
using ShopTex.Domain.Products;

namespace ShopTex.Infrastructure.OrdersProduct
{
    public class OrderProductEntityTypeConfiguration : IEntityTypeConfiguration<OrderProduct>
    {
        public void Configure(EntityTypeBuilder<OrderProduct> builder)
        {
            builder.HasKey(op => new { op.OrderId, op.ProductId });

            builder.Property(op => op.ProductId)
                .HasConversion(
                    id  => id.AsString(),       // VO → string (36 chars)
                    str => new ProductId(str))  // string → VO
                .IsRequired();

            builder.Property(op => op.OrderId)
                .HasConversion(
                    id  => id.AsString(),
                    str => new OrderId(str))
                .IsRequired();

            builder.Property(op => op.Amount).IsRequired();
            builder.Property(op => op.Price).IsRequired();

            builder.HasOne(op => op.Order)
                .WithMany(o => o.Products)
                .HasForeignKey(op => op.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(op => op.Product)
                .WithMany()
                .HasForeignKey(op => op.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}