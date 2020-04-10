using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    public interface IExceptionTask : ITask
    {
        Task Run(Exception exception, CancellationToken cancellationToken = default);
    }
}