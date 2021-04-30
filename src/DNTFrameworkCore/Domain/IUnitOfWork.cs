using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Domain
{
    public interface IUnitOfWork : IDisposable, IScopedDependency
    {
        Task Complete(CancellationToken cancellationToken = default);
    }
}