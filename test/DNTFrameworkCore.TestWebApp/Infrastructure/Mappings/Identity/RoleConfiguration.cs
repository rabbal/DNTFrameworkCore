using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Identity
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(a => a.Name).IsRequired().HasMaxLength(Role.MaxNameLength);
            builder.Property(a => a.NormalizedName).IsRequired().HasMaxLength(Role.MaxNameLength);
            builder.Property(a => a.Description).HasMaxLength(Role.MaxDescriptionLength);

            builder.HasIndex(a => a.NormalizedName).HasName("UIX_Role_NormalizedName").IsUnique();

            builder.HasMany(a => a.Users).WithOne(a => a.Role).HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.Claims).WithOne(a => a.Role).HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(a => a.Permissions).WithOne(a => a.Role).HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable(nameof(Role));
        }
    }
}