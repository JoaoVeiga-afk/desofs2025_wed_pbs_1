using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Orders;
using ShopTex.Domain.Users;

namespace ShopTex.Infrastructure.Orders;

public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.AsString(),
                str => new OrderId(str)
            )
            .ValueGeneratedNever();

        builder.Property(o => o.UserId)
            .HasConversion(
                id => id.AsString(),
                str => new UserId(str)
            )
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt).IsRequired();

        builder.OwnsOne(o => o.Status, s =>
            s.Property(x => x.Value)
                .HasColumnName(nameof(OrderStatus))
                .IsRequired());

        builder.HasMany(o => o.Products)
            .WithOne(op => op.Order)
            .HasForeignKey(op => op.OrderId)
            .HasPrincipalKey(o => o.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
