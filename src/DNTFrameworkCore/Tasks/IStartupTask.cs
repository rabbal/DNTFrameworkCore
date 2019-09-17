using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Tasks
{
    /// <summary>
    /// Defines the required contract for implementing a startup task.
    /// </summary>
    public interface IStartupTask
    {
        /// <summary>
        /// Fired when the application is starting up.
        /// </summary>
        /// <param name="cancellationToken">[Optional] The cancellation token.</param>
        /// <returns>The task instance.</returns>
        Task RunAsync(CancellationToken cancellationToken = default);
    }
}