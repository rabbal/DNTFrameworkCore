using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Identity
{
    public class UserClaimConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.Property(a => a.ClaimType).IsRequired().HasMaxLength(RoleClaim.MaxClaimTypeLength);
            builder.Property(a => a.ClaimValue).IsRequired();

            builder.ToTable(nameof(UserClaim));
        }
    }
}