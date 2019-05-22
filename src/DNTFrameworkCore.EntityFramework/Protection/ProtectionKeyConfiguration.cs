using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Protection
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyProtectionKeyConfiguration(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ProtectionKeyConfiguration());
        }
    }

    public class ProtectionKeyConfiguration : IEntityTypeConfiguration<ProtectionKey>
    {
        public void Configure(EntityTypeBuilder<ProtectionKey> builder)
        {
            builder.ToTable(nameof(ProtectionKey), "dbo");

            builder.Property(a => a.FriendlyName).IsRequired();
            builder.HasIndex(a => a.FriendlyName).IsUnique().HasName("IX_ProtectionKey_FriendlyName");
        }
    }
}