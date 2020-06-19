using DNTFrameworkCore.TestCqrsAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure.Mappings.Identity
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasDiscriminator<string>("Entity");
            builder.Property(a => a.Name).HasMaxLength(Permission.MaxNameLength).IsRequired();
            builder.Property<string>("Entity").HasMaxLength(50);

            builder.HasIndex("Entity").HasName("IX_Permission_Entity");
            builder.HasIndex("Entity", "Name").HasName("UIX_Permission_Entity_Name").IsUnique();

            builder.ToTable(nameof(Permission));
        }
    }
}