using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DNTFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DNTFrameworkCore.EFCore.Context
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EFCore
    {
        public const string CreatorBrowserName = nameof(CreatorBrowserName);
        public const string CreatorIp = nameof(CreatorIp);
        public const string CreationDateTime = nameof(CreationDateTime);
        public const string CreatorUserId = nameof(CreatorUserId);

        public const string ModifierBrowserName = nameof(ModifierBrowserName);
        public const string ModifierIp = nameof(ModifierIp);
        public const string ModificationDateTime = nameof(ModificationDateTime);
        public const string ModifierUserId = nameof(ModifierUserId);

        public const string UserId = nameof(UserId);
        public const string TenantId = nameof(TenantId);
        public const string IsDeleted = nameof(IsDeleted);
        public const string Version = nameof(Version);
        public const string Hash = nameof(Hash);

        public static readonly Func<object, string> PropertyCreatorBrowserName =
            entity => EF.Property<string>(entity, CreatorBrowserName);

        public static readonly Func<object, string> PropertyCreatorIp =
            entity => EF.Property<string>(entity, CreatorIp);

        public static readonly Func<object, DateTime> PropertyCreationDateTime =
            entity => EF.Property<DateTime>(entity, CreationDateTime);

        public static readonly Func<object, string> PropertyModifierBrowserName =
            entity => EF.Property<string>(entity, ModifierBrowserName);

        public static readonly Func<object, string> PropertyModifierIp =
            entity => EF.Property<string>(entity, ModifierIp);

        public static readonly Func<object, DateTime?> PropertyModificationDateTime =
            entity => EF.Property<DateTime?>(entity, ModificationDateTime);

        public static void AddTrackingFields<TUserId>(this ModelBuilder builder) where TUserId : IEquatable<TUserId>
        {
            var types = builder.Model.GetEntityTypes().ToList();

            var propertyType = typeof(TUserId).IsValueType
                ? typeof(Nullable<>).MakeGenericType(typeof(TUserId))
                : typeof(TUserId);

            foreach (var entityType in types.Where(e => typeof(ICreationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTime>(CreationDateTime)
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Entity(entityType.ClrType)
                    .Property<string>(CreatorBrowserName).HasMaxLength(1024)
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Entity(entityType.ClrType)
                    .Property<string>(CreatorIp).HasMaxLength(256)
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Entity(entityType.ClrType)
                    .Property(propertyType, CreatorUserId)
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }

            foreach (var entityType in types.Where(e => typeof(IModificationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTime?>(ModificationDateTime);

                builder.Entity(entityType.ClrType)
                    .Property<string>(ModifierBrowserName).HasMaxLength(1024);

                builder.Entity(entityType.ClrType)
                    .Property<string>(ModifierIp).HasMaxLength(256);

                builder.Entity(entityType.ClrType)
                    .Property(propertyType, ModifierUserId);
            }
        }

        public static void AddTenancyField<TTenantId>(this ModelBuilder builder) where TTenantId : IEquatable<TTenantId>
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(typeof(TTenantId), TenantId).Metadata
                    .SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Entity(entityType.ClrType)
                    .HasIndex(TenantId)
                    .HasName($"IX_{entityType.ClrType.Name}_{TenantId}");
            }
        }

        public static void AddIsDeletedField(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IDeletedEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<bool>(IsDeleted);
            }
        }

        public static void AddRowLevelSecurityField<TUserId>(this ModelBuilder builder)
            where TUserId : IEquatable<TUserId>
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IHasRowLevelSecurity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(typeof(TUserId), UserId)
                    .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }
        }

        public static void AddRowVersionField(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IHasRowVersion).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<byte[]>(Version)
                    .IsRowVersion();
            }
        }

        public static void AddRowIntegrityField(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IHasRowIntegrity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<string>(Hash)
                    .IsRequired()
                    .HasMaxLength(256);
            }
        }
    }
}