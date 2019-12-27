using DNTFrameworkCore.TestTenancy.Domain.Identity;

namespace DNTFrameworkCore.TestTenancy.Infrastructure.Mappings.Identity
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