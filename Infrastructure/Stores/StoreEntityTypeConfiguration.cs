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
        builder.HasKey(s => s.Id);

        builder.OwnsOne(s => s.Address, address =>
        {
            address.Property(p => p.ToString()).HasColumnName(nameof(StoreAddress)).IsRequired();
        });

        builder.OwnsOne(s => s.Status, status =>
        {
            status.Property(s => s.Value).HasColumnName(nameof(StoreStatus)).IsRequired();
        });
    }
}
