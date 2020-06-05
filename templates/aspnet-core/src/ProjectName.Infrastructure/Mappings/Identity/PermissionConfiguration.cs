using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Identity;

namespace ProjectName.Infrastructure.Mappings.Identity
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.Property(a => a.Name).HasMaxLength(Permission.NameLength).IsRequired();
            builder.Property<string>("Discriminator").HasMaxLength(50);

            builder.HasIndex("Discriminator").HasName("IX_Permission_Discriminator");

            builder.ToTable(nameof(Permission));
        }
    }
}