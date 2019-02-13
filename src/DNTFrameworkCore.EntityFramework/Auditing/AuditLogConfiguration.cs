using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Auditing
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyAuditLogConfiguration(this ModelBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.ApplyConfiguration(new AuditLogConfiguration());
        }
    }

    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable(nameof(AuditLog), "dbo");

            builder.Property(a => a.UserIp).HasMaxLength(20);
            builder.Property(a => a.UserBrowserName).HasMaxLength(1024);
            builder.Property(a => a.MethodName).HasMaxLength(256).IsRequired();
            builder.Property(a => a.ServiceName).HasMaxLength(256).IsRequired();
        }
    }
}