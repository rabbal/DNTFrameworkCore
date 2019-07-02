using System;
using DNTFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EFCore.Configuration
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyConfigurationValue(this ModelBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.ApplyConfiguration(new ConfigurationValueMap());
        }
    }

    public class ConfigurationValueMap : IEntityTypeConfiguration<ConfigurationValue>
    {
        public void Configure(EntityTypeBuilder<ConfigurationValue> builder)
        {
            builder.Property(v => v.Key).HasMaxLength(450).IsRequired();
            builder.Property(v => v.Value).IsRequired();

            builder.HasIndex(v => v.Key).IsUnique();

            builder.ToTable("Values");
        }
    }
}