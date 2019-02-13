using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Mappings.Identity
{
    public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.Property(a => a.ClaimType).IsRequired().HasMaxLength(RoleClaim.MaxClaimTypeLength);
            builder.Property(a => a.ClaimValue).IsRequired();

            builder.ToTable(nameof(RoleClaim));
        }
    }
}