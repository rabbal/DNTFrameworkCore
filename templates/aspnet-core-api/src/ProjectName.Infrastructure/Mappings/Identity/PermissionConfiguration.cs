using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Identity;

namespace ProjectName.Infrastructure.Mappings.Identity
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasDiscriminator<string>("EntityName");
            builder.Property(a => a.Name).HasMaxLength(Permission.NameLength).IsRequired();
            builder.Property<string>("EntityName").HasMaxLength(50);

            builder.HasIndex("EntityName").HasDatabaseName("IX_Permission_EntityName");

            builder.ToTable(nameof(Permission));
        }
    }
}