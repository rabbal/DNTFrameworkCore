using System.Threading.Tasks;

namespace DNTFrameworkCore.Infrastructure.Tasks
{
    public interface IBeginRequestTask 
    {
        Task ExecuteAsync();
    }
}