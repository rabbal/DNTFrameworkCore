using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace DNTFrameworkCore.EntityFramework.Context
{
    public interface
        IUnitOfWork : IDisposable, IScopedDependency
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void ApplyChanges(ITrackedEntity root);
        void AcceptChanges(ITrackedEntity root);

        int ExecuteSqlCommand(string query);
        int ExecuteSqlCommand(string query, params object[] parameters);
        Task<int> ExecuteSqlCommandAsync(string query);
        Task<int> ExecuteSqlCommandAsync(string query, params object[] parameters);

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        DbTransaction Transaction { get; }
        DbConnection Connection { get; }
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void UseTransaction(DbTransaction transaction);
        void ChangeConnectionString(string connectionString);

        bool DeleteFilterEnabled { get; set; }
        bool TenantFilterEnabled { get; set; }
        IDisposable UseTenantId(long tenantId);
        long TenantId { get; }
    }
}