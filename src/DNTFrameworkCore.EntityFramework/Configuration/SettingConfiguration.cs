using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    public static class ModelBuilderExtensions
    {
        public static void ApplySettingConfiguration(this ModelBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.ApplyConfiguration(new SettingConfiguration());
        }
    }

    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            ((EntityTypeBuilder) builder).ToTable(nameof(Setting), "dbo");

            builder.Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(a => a.Name).HasName("IX_Setting_Name");
        }
    }
}