using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyLogConfiguration(this ModelBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.ApplyConfiguration(new LogConfiguration());
        }
    }

    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.ToTable(nameof(Log), "dbo");

            builder.HasIndex(e => e.Logger).HasName("IX_Log_Logger");
            builder.HasIndex(e => e.Level).HasName("IX_Log_Level");

            builder.Property(a => a.Level).HasMaxLength(50).IsRequired();
            builder.Property(a => a.Message).IsRequired();
            builder.Property(a => a.Logger).HasMaxLength(256).IsRequired();
        }
    }
}