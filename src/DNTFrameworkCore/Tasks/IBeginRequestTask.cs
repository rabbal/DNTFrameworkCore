using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    public interface IBeginRequestTask : ITask
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}