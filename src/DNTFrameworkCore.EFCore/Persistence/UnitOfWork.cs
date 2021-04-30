using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;

namespace DNTFrameworkCore.EFCore.Persistence
{
    internal sealed class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly IDbContext _dbContext;
        private readonly IEventBus _bus;

        public UnitOfWork(IDbContext dbContext, IEventBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;
        }

        public Task Complete(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }

        ~UnitOfWork() => Dispose(false);
    }
}