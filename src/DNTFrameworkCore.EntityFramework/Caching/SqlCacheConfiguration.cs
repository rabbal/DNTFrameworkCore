using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Caching
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySqlCacheConfiguration(this ModelBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.ApplyConfiguration(new CacheConfiguration());
        }
    }

    public class CacheConfiguration : IEntityTypeConfiguration<Cache>
    {
        public void Configure(EntityTypeBuilder<Cache> builder)
        {
            builder.ToTable(name: "Cache", schema: "dbo");
            builder.HasIndex(e => e.ExpiresAtTime).HasName("IX_Cache_ExpiresAtTime");
            builder.Property(e => e.Id).HasMaxLength(449);
            builder.Property(e => e.Value).IsRequired();
        }
    }
}