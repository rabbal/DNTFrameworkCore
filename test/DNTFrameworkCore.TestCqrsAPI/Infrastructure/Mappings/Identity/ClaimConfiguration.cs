using DNTFrameworkCore.TestCqrsAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure.Mappings.Identity
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.Property(a => a.Type).IsRequired().HasMaxLength(Claim.MaxTypeLength);
            builder.Property(a => a.Value).IsRequired();

            builder.HasDiscriminator<string>("Entity");
            builder.Property<string>("Entity").HasMaxLength(50);

            builder.HasIndex("Entity").HasName("IX_Claim_Entity");
            builder.HasIndex("Entity", "Type").HasName("UIX_Claim_Entity_Type").IsUnique();

            builder.ToTable(nameof(Claim));
        }
    }
}