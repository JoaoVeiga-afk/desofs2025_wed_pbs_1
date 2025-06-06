using System.Formats.Asn1;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Users;
using ShopTex.Domain.Stores;

namespace ShopTex.Infrastructure.Users;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // StoreId mapping
        builder
            .Property(u => u.Store)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? new StoreId(v) : null
            )
            .HasColumnName(nameof(StoreId))
            .IsRequired(false);

        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(p => p.Value).HasColumnName(nameof(UserEmail)).IsRequired();
            email.HasIndex("Value").IsUnique();
        });

        builder.OwnsOne(u => u.Role, role =>
        {
            role.Property(r => r.RoleName).HasColumnName(nameof(UserRole)).IsRequired(false);
        });

        builder.OwnsOne(u => u.Status, status =>
        {
            status.Property(s => s.Value).HasColumnName(nameof(UserStatus)).IsRequired();
        });
    }
}
