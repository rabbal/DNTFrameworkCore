using DNTFrameworkCore.TestTenancy.Domain.Identity;

namespace DNTFrameworkCore.TestTenancy.Infrastructure.Mappings.Identity
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.Property(a => a.TokenHash).HasMaxLength(UserToken.MaxTokenHashLength).IsRequired();

            builder.HasIndex(a => a.TokenHash).HasName("IX_UserToken_TokenHash");
            
            builder.ToTable(nameof(UserToken));
        }
    }
}