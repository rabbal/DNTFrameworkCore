using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DNTFrameworkCore.EFCore.Context
{
    // ReSharper disable once InconsistentNaming
    public static class EFCore
    {
        private static readonly MethodInfo FilterEntityMethodInfo =
            typeof(EFCore).GetMethod(nameof(FilterEntity),
                BindingFlags.Static | BindingFlags.NonPublic);

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

        public static void AddTrackingFields<TUserId>(this ModelBuilder builder) where TUserId : IEquatable<TUserId>
        {
            var types = builder.Model.GetEntityTypes().ToList();

            var propertyType = typeof(TUserId).IsValueType
                ? typeof(Nullable<>).MakeGenericType(typeof(TUserId))
                : typeof(TUserId);

            foreach (var entityType in types.Where(e => typeof(ICreationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset>(CreationDateTime)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property<string>(CreatorBrowserName).HasMaxLength(1024)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property<string>(CreatorIp).HasMaxLength(256)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property(propertyType, CreatorUserId)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            }

            foreach (var entityType in types.Where(e => typeof(IModificationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset?>(ModificationDateTime);

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
                    .AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .HasIndex(TenantId)
                    .HasName($"IX_{entityType.ClrType.Name}_{TenantId}");
//Todo:                
//                builder.Entity(entityType.ClrType).HasKey("Id").ForSqlServerIsClustered(false);
//                builder.Entity(entityType.ClrType).HasIndex("TenantId").HasName("IX_{entityType.ClrType.Name}_TenantId")
//                    .ForSqlServerIsClustered().IsUnique(false);
            }
        }

        public static void AddSoftDeletedField(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(ISoftDeleteEntity).IsAssignableFrom(e.ClrType)))
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
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
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

        public class EntityFilteringOptions<TTenantId, TUserId>
        {
            public Func<TTenantId> TenantId { get; set; }
            public Func<TUserId> UserId { get; set; }
            public Func<bool> TenantFilterEnabled { get; set; }
            public Func<bool> DeleteFilterEnabled { get; set; }
            public Func<bool> RowLevelSecurityEnabled { get; set; }
        }

        //Under development
        public static void AddEntityFiltering<TTenantId, TUserId>(this ModelBuilder modelBuilder,
            Action<EntityFilteringOptions<TTenantId, TUserId>> options)
            where TUserId : IEquatable<TUserId>
            where TTenantId : IEquatable<TTenantId>
        {
            var types = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in types)
            {
                FilterEntityMethodInfo
                    .MakeGenericMethod(entityType.ClrType, typeof(TUserId), typeof(TTenantId))
                    .Invoke(null, new object[] {modelBuilder, entityType, options});
            }
        }

        private static void FilterEntity<TEntity, TUserId, TTenantId>(ModelBuilder modelBuilder,
            IMutableEntityType entityType, Action<EntityFilteringOptions<TTenantId, TUserId>> options)
            where TUserId : IEquatable<TUserId>
            where TTenantId : IEquatable<TTenantId>
            where TEntity : class
        {
            if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>()) return;

            var filterExpression =
                BuildFilterExpression<TEntity, TUserId, TTenantId>(options);
            if (filterExpression == null) return;

            if (entityType.IsQueryType)
            {
                modelBuilder.Query<TEntity>().HasQueryFilter(filterExpression);
            }
            else
            {
                modelBuilder.Entity<TEntity>().HasQueryFilter(filterExpression);
            }
        }

        private static bool ShouldFilterEntity<TEntity>() where TEntity : class
        {
            return typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)) ||
                   typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)) ||
                   typeof(IHasRowLevelSecurity).IsAssignableFrom(typeof(TEntity));
        }

        private static Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity, TUserId, TTenantId>(
            Action<EntityFilteringOptions<TTenantId, TUserId>> optionSetup)
            where TUserId : IEquatable<TUserId>
            where TTenantId : IEquatable<TTenantId>
            where TEntity : class
        {
            var options = new EntityFilteringOptions<TTenantId, TUserId>();
            optionSetup.Invoke(options);

            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> deleteFilterExpression =
                    e => !options.DeleteFilterEnabled() || !EF.Property<bool>(e, IsDeleted);

                expression = deleteFilterExpression;
            }

            if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> tenantFilterExpression =
                    e => !options.TenantFilterEnabled() ||
                         EF.Property<TTenantId>(e, TenantId).Equals(options.TenantId());

                expression = expression == null
                    ? tenantFilterExpression
                    : expression.Combine(tenantFilterExpression);
            }

            if (!typeof(IHasRowLevelSecurity).IsAssignableFrom(typeof(TEntity))) return expression;

            Expression<Func<TEntity, bool>>
                rlsFilterExpression = e => !options.RowLevelSecurityEnabled() ||
                                           EF.Property<TUserId>(e, UserId).Equals(options.UserId());

            expression = expression == null
                ? rlsFilterExpression
                : expression.Combine(rlsFilterExpression);

            return expression;
        }
    }
}