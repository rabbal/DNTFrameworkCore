using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;
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
        public bool HasTransaction => Transaction != null;
        public IDbContextTransaction Transaction { get; private set; }

        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (HasTransaction) return Transaction;

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
            if (!HasTransaction) throw new NullReferenceException("Please call `BeginTransaction()` method first.");

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
            if (!HasTransaction) throw new NullReferenceException("Please call `BeginTransaction()` method first.");

            try
            {
                Transaction.Rollback();
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
            catch (DbUpdateException e)
            {
                throw new DbException(e.Message, e);
            }

            return result;
        }

        public int ExecuteSqlInterpolatedCommand(FormattableString query)
        {
            return Database.ExecuteSqlInterpolated(query);
        }

        public int ExecuteSqlRawCommand(string query, params object[] parameters)
        {
            return Database.ExecuteSqlRaw(query, parameters);
        }

        public Task<int> ExecuteSqlInterpolatedCommandAsync(FormattableString query)
        {
            return Database.ExecuteSqlInterpolatedAsync(query);
        }

        public Task<int> ExecuteSqlRawCommandAsync(string query, params object[] parameters)
        {
            return Database.ExecuteSqlRawAsync(query, parameters);
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