using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using DbUpdateException = Microsoft.EntityFrameworkCore.DbUpdateException;

namespace DNTFrameworkCore.EFCore.Context
{
    public abstract class DbContextCore : DbContext, IDbContext
    {
        private readonly IHookEngine _hookEngine;

        protected DbContextCore(IHookEngine hookEngine, DbContextOptions options) : base(options)
        {
            _hookEngine = hookEngine ?? throw new ArgumentNullException(nameof(hookEngine));
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
            builder.AddNumberedEntity();
        }
    }
}