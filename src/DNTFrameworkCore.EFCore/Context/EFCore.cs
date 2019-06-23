using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DNTFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DNTFrameworkCore.EFCore.Context
{
    // ReSharper disable once InconsistentNaming
    public static class EFCore
    {
        private static readonly MethodInfo QueryFiltersMethodInfo =
            typeof(DbContextCore).GetMethod(nameof(ConfigureQueryFilters),
                BindingFlags.Static | BindingFlags.NonPublic);

        public static readonly Func<object, string> CreatorBrowserName =
            entity => EF.Property<string>(entity, nameof(CreatorBrowserName));

        public static readonly Func<object, string> CreatorIp =
            entity => EF.Property<string>(entity, nameof(CreatorIp));

        public static readonly Func<object, DateTimeOffset> CreationDateTime =
            entity => EF.Property<DateTimeOffset>(entity, nameof(CreationDateTime));

        public static readonly Func<object, long?> CreatorUserId =
            entity => EF.Property<long?>(entity, nameof(CreatorUserId));

        public static readonly Func<object, DateTimeOffset?> ModificationDateTime =
            entity => EF.Property<DateTimeOffset?>(entity, nameof(ModificationDateTime));

        public static readonly Func<object, string> ModifierIp =
            entity => EF.Property<string>(entity, nameof(ModifierIp));

        public static readonly Func<object, string> ModifierBrowserName =
            entity => EF.Property<string>(entity, nameof(ModifierBrowserName));

        public static readonly Func<object, long?> ModifierUserId =
            entity => EF.Property<long?>(entity, nameof(ModifierUserId));

        public static readonly Func<object, long> UserId =
            entity => EF.Property<long>(entity, nameof(UserId));

        public static readonly Func<object, long> TenantId =
            entity => EF.Property<long>(entity, nameof(TenantId));

        public static readonly Func<object, bool> IsDeleted =
            entity => EF.Property<bool>(entity, nameof(IsDeleted));

        public static readonly Func<object, byte[]> RowVersion =
            entity => EF.Property<byte[]>(entity, nameof(RowVersion));

        public static void AddTracking(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(ICreationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset>(nameof(CreationDateTime))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property<string>(nameof(CreatorBrowserName)).HasMaxLength(1024)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property<string>(nameof(CreatorIp)).HasMaxLength(256)
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .Property<long?>(nameof(CreatorUserId))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            }

            foreach (var entityType in types.Where(e => typeof(IModificationTracking).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<DateTimeOffset?>(nameof(ModificationDateTime));

                builder.Entity(entityType.ClrType)
                    .Property<string>(nameof(ModifierBrowserName)).HasMaxLength(1024);

                builder.Entity(entityType.ClrType)
                    .Property<string>(nameof(ModifierIp)).HasMaxLength(256);

                builder.Entity(entityType.ClrType)
                    .Property<long?>(nameof(ModifierUserId));
            }
        }

        public static void AddTenantEntity(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(ITenantEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<long>(nameof(TenantId)).Metadata
                    .AfterSaveBehavior = PropertySaveBehavior.Ignore;

                builder.Entity(entityType.ClrType)
                    .HasIndex(nameof(TenantId))
                    .HasName($"IX_{entityType.ClrType.Name}_{nameof(TenantId)}");
            }
        }

        public static void AddSoftDeleteEntity(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(ISoftDeleteEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<bool>(nameof(IsDeleted));
            }
        }

        public static void AddRowLevelSecurity(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IHasRowLevelSecurity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property<long>(nameof(UserId))
                    .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            }
        }

        public static void AddRowVersion(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(IHasRowVersion).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(RowVersion))
                    .IsRowVersion();
            }
        }

        public static void AddNumberedEntity(this ModelBuilder builder)
        {
            var types = builder.Model.GetEntityTypes().ToList();

            foreach (var entityType in types.Where(e => typeof(INumberedEntity).IsAssignableFrom(e.ClrType)))
            {
                builder.Entity(entityType.ClrType)
                    .Property(nameof(INumberedEntity.Number)).IsRequired().HasMaxLength(50).Metadata
                    .AfterSaveBehavior = PropertySaveBehavior.Ignore;

                if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    builder.Entity(entityType.ClrType)
                        .HasIndex(nameof(INumberedEntity.Number), nameof(TenantId))
                        .HasName(
                            $"UIX_{entityType.ClrType.Name}_{nameof(TenantId)}_{nameof(INumberedEntity.Number)}")
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
        }

        public static void ApplyQueryFilters(this ModelBuilder modelBuilder, long tenantId, long userId)
        {
            var types = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in types)
            {
                QueryFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(null, new object[] {modelBuilder, entityType});
            }
        }

        private static void ConfigureQueryFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
            where TEntity : class
        {
            if (entityType.BaseType != null || !ShouldFilterEntity<TEntity>()) return;

            var filterExpression = BuildFilterExpression<TEntity>();
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

        private static Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;
//
//            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
//            {
//                Expression<Func<TEntity, bool>> deleteFilterExpression = e =>
//                    !DeleteFilterEnabled || !EF.Property<bool>(e, "IsDeleted");
//
//                expression = deleteFilterExpression;
//            }
//
//            if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
//            {
//                Expression<Func<TEntity, bool>> tenantFilterExpression = e =>
//                    !TenantFilterEnabled || EF.Property<long>(e, "TenantId") == _tenantId;
//
//                expression = expression == null
//                    ? tenantFilterExpression
//                    : expression.Combine(tenantFilterExpression);
//            }
//
//            if (!typeof(IHasRowLevelSecurity).IsAssignableFrom(typeof(TEntity))) return expression;
//
//            Expression<Func<TEntity, bool>> branchFilterExpression = e =>
//                !RowLevelSecurityEnabled || EF.Property<long>(e, "UserId") == _userId;
//
//            expression = expression == null
//                ? branchFilterExpression
//                : expression.Combine(branchFilterExpression);
//
            return expression;
        }
    }
}