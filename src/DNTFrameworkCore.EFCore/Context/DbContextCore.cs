using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace DNTFrameworkCore.EFCore.Context
{
    public abstract class DbContextCore : DbContext, IUnitOfWork
    {
        private readonly IHookEngine _hookEngine;
        private readonly IUserSession _session;

        private static readonly MethodInfo FilterEntityMethodInfo =
            typeof(DbContextCore).GetMethod(nameof(FilterEntity),
                BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly long _userId;
        private readonly long _tenantId;

        protected DbContextCore(IHookEngine hookEngine, IUserSession session, DbContextOptions options) : base(options)
        {
            _hookEngine = hookEngine ?? throw new ArgumentNullException(nameof(hookEngine));
            _session = session ?? throw new ArgumentNullException(nameof(session));

            _tenantId = _session.TenantId ?? 0;
            _userId = _session.UserId ?? 0;
        }

        public DbConnection Connection => Database.GetDbConnection();
        public bool HasActiveTransaction => Transaction != null;
        public IDbContextTransaction Transaction { get; private set; }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (Transaction != null) return null;

            return Transaction = Database.BeginTransaction(isolationLevel);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (Transaction != null) return null;

            return Transaction = await Database.BeginTransactionAsync(isolationLevel);
        }

        public void CommitTransaction()
        {
            if (Transaction == null) throw new InvalidOperationException("Transaction is null!");

            try
            {
                Transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                Transaction?.Rollback();
            }
            finally
            {
                if (Transaction != null)
                {
                    Transaction.Dispose();
                    Transaction = null;
                }
            }
        }

        public bool DeleteFilterEnabled { get; set; } = true;
        public bool TenantFilterEnabled { get; set; } = true;
        public bool RowLevelSecurityEnabled { get; set; } = true;

        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

        public void UseConnectionString(string connectionString)
        {
            Database.GetDbConnection().ConnectionString = connectionString;
        }

        public int ExecuteSqlCommand(string query)
        {
            return Database.ExecuteSqlCommand(query);
        }

        public int ExecuteSqlCommand(string query, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(query, parameters);
        }

        public void TrackGraph<TEntity>(TEntity entity, Action<EntityEntryGraphNode> callback) where TEntity : class
        {
            ChangeTracker.TrackGraph(entity, callback);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();

            int result;
            try
            {
                var names = this.FindChangedEntityNames();
                var entries = this.FindChangedEntries();

                _hookEngine.RunPreHooks(entries);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = await base.SaveChangesAsync(true, cancellationToken);
                ChangeTracker.AutoDetectChangesEnabled = true;

                _hookEngine.RunPostHooks(entries);

                AfterSaveChanges(new EntityChangeContext(names));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new Exceptions.DbUpdateException(e.Message, e);
            }

            return result;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            int result;
            try
            {
                var names = this.FindChangedEntityNames();
                var entryList = this.FindChangedEntries();

                _hookEngine.RunPreHooks(entryList);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = base.SaveChanges(true);
                ChangeTracker.AutoDetectChangesEnabled = true;

                _hookEngine.RunPostHooks(entryList);

                AfterSaveChanges(new EntityChangeContext(names));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new ConcurrencyException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new Exceptions.DbUpdateException(e.Message, e);
            }

            return result;
        }

        public Task<int> ExecuteSqlCommandAsync(string query)
        {
            return Database.ExecuteSqlCommandAsync(query);
        }

        public Task<int> ExecuteSqlCommandAsync(string query, params object[] parameters)
        {
            return Database.ExecuteSqlCommandAsync(query, parameters);
        }

        protected virtual void AfterSaveChanges(EntityChangeContext context)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.AddTracking();
            builder.AddTenantEntity();
            builder.AddSoftDeleteEntity();
            builder.AddRowVersion();
            builder.AddRowLevelSecurity();
            
            ApplyFilters(builder);
        }

        private void ApplyFilters(ModelBuilder modelBuilder)
        {
            var types = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in types)
            {
                FilterEntityMethodInfo
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(this, new object[] {modelBuilder, entityType});
            }
        }

        private void FilterEntity<TEntity>(ModelBuilder modelBuilder, IMutableEntityType entityType)
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

        private Expression<Func<TEntity, bool>> BuildFilterExpression<TEntity>()
            where TEntity : class
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> deleteFilterExpression = e =>
                    !DeleteFilterEnabled || !EF.Property<bool>(e, EFCore.IsDeleted);

                expression = deleteFilterExpression;
            }

            if (typeof(ITenantEntity).IsAssignableFrom(typeof(TEntity)))
            {
                Expression<Func<TEntity, bool>> tenantFilterExpression = e =>
                    !TenantFilterEnabled || EF.Property<long>(e, EFCore.TenantId) == _tenantId;

                expression = expression == null
                    ? tenantFilterExpression
                    : expression.Combine(tenantFilterExpression);
            }

            if (!typeof(IHasRowLevelSecurity).IsAssignableFrom(typeof(TEntity))) return expression;

            Expression<Func<TEntity, bool>> rlsFilterExpression = e =>
                !RowLevelSecurityEnabled || EF.Property<long>(e, EFCore.UserId) == _userId;

            expression = expression == null
                ? rlsFilterExpression
                : expression.Combine(rlsFilterExpression);

            return expression;
        }
    }
}