using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    /// <summary>
    /// Defines the required contract for implementing a shutdown task.
    /// </summary>
    public interface IShutdownTask
    {
        /// <summary>
        /// Fired when the application is shutting down.
        /// </summary>
        /// <param name="cancellationToken">[Optional] The cancellation token.</param>
        /// <returns>The task instance.</returns>
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}