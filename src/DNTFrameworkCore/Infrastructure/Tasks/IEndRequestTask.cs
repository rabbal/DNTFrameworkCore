using System.Threading.Tasks;

namespace DNTFrameworkCore.Infrastructure.Tasks
{
    public interface IEndRequestTask 
    {
        Task ExecuteAsync();
    }
}