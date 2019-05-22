using System.Linq;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyFrameworkConventions(this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(ICreationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(ICreationTracking.CreatorBrowserName))
                    .HasMaxLength(1024)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property(nameof(ICreationTracking.CreatorIp))
                    .HasMaxLength(256)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property(nameof(ICreationTracking.CreationDateTime))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property(nameof(ICreationTracking.CreatorUserId))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            }

            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(IModificationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(IModificationTracking.LastModifierBrowserName))
                    .HasMaxLength(1024);

                builder.Entity(entityType.ClrType)
                    .Property(nameof(IModificationTracking.LastModifierIp))
                    .HasMaxLength(256);
            }

            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(IHasRowVersion).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(IHasRowVersion.RowVersion))
                    .IsRowVersion();
            }

            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(IHasRowLevelSecurity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(IHasRowLevelSecurity.UserId))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            }

            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(INumberedEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(INumberedEntity.Number)).IsRequired().HasMaxLength(50).Metadata
                    .AfterSaveBehavior = PropertySaveBehavior.Ignore;

                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .HasIndex(nameof(INumberedEntity.Number), nameof(ITenantEntity.TenantId))
                        .HasName(
                            $"UIX_{entityType.ClrType.Name}_{nameof(ITenantEntity.TenantId)}_{nameof(INumberedEntity.Number)}")
                        .IsUnique();
                }
                else
                {
                    builder.Entity(entityType.ClrType)
                        .HasIndex(nameof(INumberedEntity.Number))
                        .HasName($"UIX_{entityType.ClrType.Name}_{nameof(INumberedEntity.Number)}")
                        .IsUnique();
                }
            }

            foreach (var entityType in builder.Model
                .GetEntityTypes()
                .Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(ITenantEntity.TenantId)).Metadata
                    .AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .HasIndex(nameof(ITenantEntity.TenantId))
                    .HasName($"IX_{entityType.ClrType.Name}_{nameof(ITenantEntity.TenantId)}");
            }
        }
    }
}