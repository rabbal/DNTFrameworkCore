using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    public interface IStartupTask : ITask
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}