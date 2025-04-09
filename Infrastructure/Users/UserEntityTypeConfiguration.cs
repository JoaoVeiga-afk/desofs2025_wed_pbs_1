using System.Formats.Asn1;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopTex.Domain.Users;

namespace ShopTex.Infrastructure.Users;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(p => p.Value).HasColumnName(nameof(UserEmail)).IsRequired();
            email.HasIndex("Value").IsUnique();
        });

        builder.OwnsOne(u => u.Role, role =>
        {
            role.Property(r => r.RoleName).HasColumnName(nameof(UserRole)).IsRequired();
        });

        builder.OwnsOne(u => u.Nif, nif =>
        {
            nif.Property(n => n.Value).HasColumnName(nameof(UserNif));
        });

        builder.OwnsOne(u => u.Status, status =>
        {
            status.Property(s => s.Value).HasColumnName(nameof(UserStatus)).IsRequired();
        });
    }
}