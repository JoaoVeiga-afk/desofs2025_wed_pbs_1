using System.Formats.Asn1;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Stores;

namespace ShopTex.Infrastructure.Stores;

public class StoreEntityTypeConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id  => id.AsString(),   // StoreId to string (for DB)
                str => new StoreId(str) // string to StoreId
            )
            .ValueGeneratedNever();

        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(p => p.Street).HasColumnName("Street").IsRequired();
            address.Property(p => p.City).HasColumnName("City").IsRequired();
            address.Property(p => p.State).HasColumnName("State").IsRequired();
            address.Property(p => p.ZipCode).HasColumnName("ZipCode").IsRequired();
            address.Property(p => p.Country).HasColumnName("Country").IsRequired();
        });

        builder.OwnsOne(s => s.Status, status =>
        {
            status.Property(s => s.Value).HasColumnName(nameof(StoreStatus)).IsRequired();
        });
    }

}
