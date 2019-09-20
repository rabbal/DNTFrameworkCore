using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using DbException = DNTFrameworkCore.Exceptions.DbException;

namespace DNTFrameworkCore.EFCore.Context
{
    public abstract class DbContextCore : DbContext, IUnitOfWork
    {
        private readonly IEnumerable<IHook> _hooks;

        protected DbContextCore(DbContextOptions options, IEnumerable<IHook> hooks) : base(options)
        {
            _hooks = hooks ?? throw new ArgumentNullException(nameof(hooks));
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
            if (Transaction == null) throw new InvalidOperationException("Transaction is null");

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
            int result;
            try
            {
                var entryList = this.FindChangedEntries();
                var names = entryList.FindEntityNames();

                ExecuteHooks<IPreActionHook>(entryList);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = await base.SaveChangesAsync(true, cancellationToken);
                ChangeTracker.AutoDetectChangesEnabled = true;

                ExecuteHooks<IPostActionHook>(entryList);

                SavedChanges(new EntityChangeContext(names, entryList));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (DbUpdateException e)
            {
                throw new DbException(e.Message, e);
            }

            return result;
        }

        public override int SaveChanges()
        {
            int result;
            try
            {
                var entryList = this.FindChangedEntries();
                var names = entryList.FindEntityNames();

                ExecuteHooks<IPreActionHook>(entryList);

                ChangeTracker.AutoDetectChangesEnabled = false;
                result = base.SaveChanges(true);
                ChangeTracker.AutoDetectChangesEnabled = true;

                ExecuteHooks<IPostActionHook>(entryList);

                SavedChanges(new EntityChangeContext(names, entryList));
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message, e);
            }
            catch (DbException e)
            {
                throw new Exceptions.DbException(e.Message, e);
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

        protected virtual void SavedChanges(EntityChangeContext context)
        {
        }

        protected virtual void ExecuteHooks<THook>(IEnumerable<EntityEntry> entryList) where THook : IHook
        {
            foreach (var entry in entryList)
            {
                var hooks = _hooks.OfType<THook>().Where(x => x.HookState == entry.State).OrderBy(hook => hook.Order);
                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entry);
                    hook.Hook(entry.Entity, metadata, this);
                }
            }
        }
    }
}