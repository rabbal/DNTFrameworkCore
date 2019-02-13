using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Mappings.Identity
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.Property(a => a.TokenHash).HasMaxLength(UserToken.MaxAccessTokenHashLength).IsRequired();

            builder.HasIndex(a => a.TokenHash).HasName("IX_UserToken_AccessTokenHash");
            
            builder.ToTable(nameof(UserToken));
        }
    }
}