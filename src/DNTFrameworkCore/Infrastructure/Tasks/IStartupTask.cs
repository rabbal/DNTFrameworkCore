using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Infrastructure.Tasks
{
    public interface IStartupTask
    {
        void ConfigureServices(IServiceCollection services);
        void Execute();
    }
}