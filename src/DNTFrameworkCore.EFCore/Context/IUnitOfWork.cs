using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context
{
    public interface IUnitOfWork : IDisposable
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
        
        Task BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
        Task CommitTransaction(CancellationToken cancellationToken = default);
        void RollbackTransaction();
    }
}