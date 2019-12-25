using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace DNTFrameworkCore.EFCore.Context
{
    public interface IUnitOfWork : IDisposable, IScopedDependency
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        string EntityHash<TEntity>(TEntity entity) where TEntity : class;
        void TrackGraph(object rootEntity, Action<EntityEntryGraphNode> callback);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        int ExecuteSqlInterpolatedCommand(FormattableString query);
        int ExecuteSqlRawCommand(string query, params object[] parameters);
        Task<int> ExecuteSqlInterpolatedCommandAsync(FormattableString query);
        Task<int> ExecuteSqlRawCommandAsync(string query, params object[] parameters);
        void UseTransaction(DbTransaction transaction);
        void UseConnectionString(string connectionString);
        bool HasTransaction { get; }
        DbConnection Connection { get; }
        IDbContextTransaction Transaction { get; }
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
        void IgnoreHook(string hookName);
    }
}