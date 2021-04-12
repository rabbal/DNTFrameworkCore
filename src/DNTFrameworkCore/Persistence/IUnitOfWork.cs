using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Persistence
{
    public interface IUnitOfWork : IDisposable, IScopedDependency
    {
        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}