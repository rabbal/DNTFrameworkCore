using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using DbException = DNTFrameworkCore.Exceptions.DbException;

namespace DNTFrameworkCore.EFCore.Context
{
    public abstract class DbContextCore : DbContext, IUnitOfWork
    {
        private readonly IEnumerable<IHook> _hooks;
        private readonly List<string> _ignoredHookList = new List<string>();

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
            if (HasTransaction) return Transaction;

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

        public void IgnoreHook(string hookName)
        {
            _ignoredHookList.Add(hookName);
        }

        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

        public void UseConnectionString(string connectionString)
        {
            Connection.ConnectionString = connectionString;
        }

        public string EntityHash<TEntity>(TEntity entity) where TEntity : class
        {
            var row = Entry(entity).ToDictionary(p => p.Metadata.Name != EFCore.Hash &&
                                                      !p.Metadata.ValueGenerated.HasFlag(ValueGenerated.OnUpdate) &&
                                                      !p.Metadata.IsShadowProperty());
            return EntityHash<TEntity>(row);
        }

        protected virtual string EntityHash<TEntity>(Dictionary<string, object> row) where TEntity : class
        {
            var json = JsonConvert.SerializeObject(row, Formatting.Indented);
            using (var hashAlgorithm = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(json);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback)
        {
            ChangeTracker.TrackGraph(rootEntity, callback);
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

                //for RowIntegrity scenarios
                await base.SaveChangesAsync(true, cancellationToken);

                OnSaveCompleted(new EntityChangeContext(names, entryList));
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

                //for RowIntegrity scenarios
                base.SaveChanges(true);

                OnSaveCompleted(new EntityChangeContext(names, entryList));
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

        protected virtual void OnSaveCompleted(EntityChangeContext context)
        {
        }

        protected virtual void ExecuteHooks<THook>(IEnumerable<EntityEntry> entryList) where THook : IHook
        {
            foreach (var entry in entryList)
            {
                var hooks = _hooks.OfType<THook>()
                    .Where(hook => !_ignoredHookList.Contains(hook.Name))
                    .Where(hook => hook.HookState == entry.State).OrderBy(hook => hook.Order);

                foreach (var hook in hooks)
                {
                    var metadata = new HookEntityMetadata(entry);
                    hook.Hook(entry.Entity, metadata, this);
                }
            }
        }
    }
}