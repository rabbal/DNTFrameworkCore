using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Tasks;
using Microsoft.Extensions.Hosting;

namespace DNTFrameworkCore.Web.Hosting
{
    internal sealed class TaskHostedService : IHostedService
    {
        private readonly ITaskEngine _engine;

        public TaskHostedService(ITaskEngine engine)
        {
            _engine = engine;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _engine.RunOnStartup(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _engine.RunOnEnd(cancellationToken);
        }
    }
}