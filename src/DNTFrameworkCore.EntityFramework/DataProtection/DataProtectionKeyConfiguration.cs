using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.DataProtection
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyDataProtectionKeyConfiguration(this ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DataProtectionKeyConfiguration());
        }
    }

    public class DataProtectionKeyConfiguration : IEntityTypeConfiguration<DataProtectionKey>
    {
        public void Configure(EntityTypeBuilder<DataProtectionKey> builder)
        {
            builder.ToTable(nameof(DataProtectionKey), "dbo");

            builder.Property(a => a.FriendlyName).IsRequired();
            builder.HasIndex(a => a.FriendlyName).IsUnique().HasName("IX_DataProtectionKey_FriendlyName");
        }
    }
}