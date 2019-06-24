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
        void TrackGraph<TEntity>(TEntity entity, Action<EntityEntryGraphNode> callback) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
        int ExecuteSqlCommand(string query);
        int ExecuteSqlCommand(string query, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string query);
        Task<int> ExecuteSqlCommandAsync(string query, params object[] parameters);
        void UseTransaction(DbTransaction transaction);
        void UseConnectionString(string connectionString);
        bool HasActiveTransaction { get; }
        DbConnection Connection { get; }
        IDbContextTransaction Transaction { get; }
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
        bool DeleteFilterEnabled { get; set; }
        bool TenantFilterEnabled { get; set; }
        bool RowLevelSecurityEnabled { get; set; }
    }
}