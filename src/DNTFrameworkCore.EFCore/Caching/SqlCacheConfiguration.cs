using System;
using DNTFrameworkCore.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EFCore.Caching
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySqlCacheConfiguration(this ModelBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new CacheConfiguration());
        }
    }

    public class CacheConfiguration : IEntityTypeConfiguration<Cache>
    {
        public void Configure(EntityTypeBuilder<Cache> builder)
        {
            builder.Property(e => e.Id).HasMaxLength(449);
            builder.Property(e => e.Value).IsRequired();
            
            builder.HasIndex(e => e.ExpiresAtTime).HasName("IX_Cache_ExpiresAtTime");
            
            builder.ToTable(name: "Cache", schema: "dbo");
        }
    }
}