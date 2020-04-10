using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    public interface IEndRequestTask : ITask
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}