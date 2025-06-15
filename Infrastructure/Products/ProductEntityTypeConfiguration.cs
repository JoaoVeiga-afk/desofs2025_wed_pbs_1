using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Products;
using ShopTex.Domain.Stores;

namespace ShopTex.Infrastructure.Products;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.AsString(),
                str => new ProductId(str)
            )
            .ValueGeneratedNever();

        builder.OwnsOne(s => s.Status, status =>
        {
            status.Property(s => s.Value)
                .HasColumnName(nameof(ProductStatus))
                .IsRequired();
        });

        builder.OwnsOne(s => s.StoreId, store =>
        {
            store.Property(s => s.Value)
                .HasColumnName(nameof(StoreId))
                .IsRequired();
        });

        builder.OwnsOne(s => s.Image, image =>
        {
            image.Property(i => i.ImagePath)
                .HasColumnName(nameof(ProductImage.ImagePath))
                .HasMaxLength(260)
                .IsRequired();

            image.Property(i => i.EncryptionKey)
                .HasColumnName(nameof(ProductImage.EncryptionKey))
                .IsRequired();

            image.Property(i => i.InitializationVector)
                .HasColumnName(nameof(ProductImage.InitializationVector))
                .IsRequired();
        });
    }
}
