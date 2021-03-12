using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectName.Domain.Identity;

namespace ProjectName.Infrastructure.Mappings.Identity
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.Property(a => a.TokenHash).HasMaxLength(UserToken.TokenHashLength).IsRequired();

            builder.HasIndex(a => a.TokenHash).HasDatabaseName("IX_UserToken_TokenHash");
            
            builder.ToTable(nameof(UserToken));
        }
    }
}