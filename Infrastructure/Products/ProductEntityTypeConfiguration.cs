using System.Formats.Asn1;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Products;
using ShopTex.Domain.Stores;

namespace ShopTex.Infrastructure.Products;

public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(s => s.Id);

        builder.OwnsOne(s => s.Status, status =>
        {
            status.Property(s => s.Value).HasColumnName(nameof(ProductStatus)).IsRequired();
        });

        builder.OwnsOne(s => s.StoreId, store =>
        {
            store.Property(s => s.Value).HasColumnName(nameof(StoreId)).IsRequired();
        });
    }
}
