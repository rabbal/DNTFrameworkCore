using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    public interface IEndTask : ITask
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}