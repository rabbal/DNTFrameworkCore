using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Extensions;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Helpers;
using DNTFrameworkCore.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using DbUpdateException = DNTFrameworkCore.Exceptions.DbUpdateException;

namespace DNTFrameworkCore.EntityFramework.Context
{
    public abstract class DbContextCore : DbContext, IUnitOfWork
    {
        private static readonly MethodInfo ConfigureQueryFiltersMethodInfo =
            typeof(DbContextCore).GetMethod(nameof(ConfigureQueryFilters),
                BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly IHookEngine _hookEngine;

        protected DbContextCore(
            IHookEngine hookEngine,
            IUserSession session,
            DbContextOptions options) : base(options)
        {
            _hookEngine = hookEngine ?? throw new ArgumentNullException(nameof(hookEngine));
            TenantId = (session ?? throw new ArgumentNullException(nameof(session))).TenantId ?? 0;
        }

        public DbTransaction Transaction => Database.CurrentTransaction?.GetDbTransaction();
        public DbConnection Connection => Database.GetDbConnection();

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return Database.CurrentTransaction ?? Database.BeginTransaction(isolationLevel);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return Database.CurrentTransaction ?? await Database.BeginTransactionAsync(isolationLevel);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

        public void ChangeConnectionString(string connectionString)
        {
            Connection.ConnectionString = connectionString;
        }

        public bool DeleteFilterEnabled { get; set; } = true;
        public bool TenantFilterEnabled { get; set; } = true;

        public IDisposable UseTenantId(long tenantId)
        {
            var oldTenantId = TenantId;
            TenantId = tenantId;

            return new DisposeAction(() => { TenantId = oldTenantId; });
        }

        public long TenantId { get; private set; }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            base.AddRange(entities);
        }

        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            base.UpdateRange(entities);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            base.RemoveRange(entities);
        }

        public void ApplyChanges(ITrackedEntity root)
        {
            ChangeTracker.TrackGraph(root, node =>
            {
                if (!(node.Entry.Entity is ITrackedEntity haveTrackingState)) return;

                node.Entry.State = EntityState.Detached;

                // Get related parent entity
                if (node.SourceEntry != null)
                {
                    var relationship = node.InboundNavigation?.GetRelationshipType();
                    switch (relationship)
                    {
                        case RelationshipType.OneToOne:
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                node.Entry.State = TrackingState.Added.ToEntityState();
                            }
                            else if (node.SourceEntry.State == EntityState.Deleted)
                            {
                                node.Entry.State = TrackingState.Deleted.ToEntityState();
                            }
                            else
                            {
                                node.Entry.State = haveTrackingState.TrackingState.ToEntityState();
                            }

                            return;
                        case RelationshipType.ManyToOne:
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                node.Entry.State = TrackingState.Added.ToEntityState();
                                return;
                            }

                            var parent = node.SourceEntry.Entity as ITrackedEntity;
                            if (node.SourceEntry.State == EntityState.Deleted
                                || parent?.TrackingState == TrackingState.Deleted)
                            {
                                try
                                {
                                    node.Entry.State = TrackingState.Deleted.ToEntityState();
                                }
                                catch (InvalidOperationException e)
                                {
                                    throw new InvalidOperationException(
                                        @"An entity may not be marked as Deleted if it has related entities which are marked as Added.
                                            Remove added related entities before deleting a parent entity.",
                                        e);
                                }

                                return;
                            }

                            break;
                        case RelationshipType.OneToMany:
                            // If ITrackedEntity is set deleted set entity state to unchanged,
                            // since it may be related to other entities.
                            if (haveTrackingState.TrackingState == TrackingState.Deleted)
                            {
                                node.Entry.State = TrackingState.Unchanged.ToEntityState();
                                return;
                            }

                            break;
                    }
                }

                node.Entry.State = haveTrackingState.TrackingState.ToEntityState();
            });
        }

        public void AcceptChanges(ITrackedEntity root)
        {
            this.TraverseGraph(root, n =>
            {
                if (!(n.Entry.Entity is ITrackedEntity haveTrackingState)) return;

                if (haveTrackingState.TrackingState != TrackingState.Unchanged)
                    haveTrackingState.TrackingState = TrackingState.Unchanged;
            });
        }

        public int ExecuteSqlCommand(string query)
        {
            return Database.ExecuteSqlCommand(query);
        }

        public int ExecuteSqlCommand(string query, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(query, parameters);
        }

        public Task<int> ExecuteSqlCommandAsync(string query)
        {
            return Database.ExecuteSqlCommandAsync(query);
        }

        public Task<int> ExecuteSqlCommandAsync(string query, params object[] parameters)
        {
            return Database.ExecuteSqlCommandAsync(query, parameters);
        }

        public override int SaveChanges()
        {
            BeforeSaveChanges();

            int result;
            try
            {
                var names = this.FindChangedEntityNames();
                var entries = this.FindChangedEntries();

                _hookEngine.ExecutePreActionHooks(entries);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = base.SaveChanges(true);
                ChangeTracker.AutoDetectChangesEnabled = true;

                _hookEngine.ExecutePostActionHooks(entries);

                AfterSaveChanges(new SaveChangeContext(names));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                throw new DbUpdateException(e.Message, e);
            }

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            BeforeSaveChanges();

            int result;
            try
            {
                var names = this.FindChangedEntityNames();
                var entries = this.FindChangedEntries();

                _hookEngine.ExecutePreActionHooks(entries);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = await base.SaveChangesAsync(true, cancellationToken);
                ChangeTracker.AutoDetectChangesEnabled = true;

                _hookEngine.ExecutePostActionHooks(entries);

                AfterSaveChanges(new SaveChangeContext(names));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                throw new DbUpdateException(e.Message, e);
            }

            return result;
        }

        protected virtual void AfterSaveChanges(SaveChangeContext context)
        {
        }

        protected virtual void BeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            this.ValidateEntities();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyFrameworkConventions();

            ApplyQueryFilters(modelBuilder);
        }

        private void ApplyQueryFilters(ModelBuilder modelBuilder)
        {
            var types = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in types)
            {
                ConfigureQueryFiltersMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] { modelBuilder, entityType });
            }
        }

        private void ConfigureQueryFilters<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
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
                   typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity));
        }

        private Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> deleteFilterExpression = e =>
                    !DeleteFilterEnabled || !((ISoftDeleteEntity)e).IsDeleted;

                expression = deleteFilterExpression;
            }

            if (!typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity))) return expression;

            Expression<Func<TEntity, bool>> tenantFilterExpression = e =>
                !TenantFilterEnabled || ((ITenantEntity)e).TenantId == TenantId;

            expression = expression == null
                ? tenantFilterExpression
                : expression.Combine(tenantFilterExpression);

            return expression;
        }
    }
}