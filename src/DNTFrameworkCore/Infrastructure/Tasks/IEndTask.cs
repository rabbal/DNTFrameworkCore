using System.Threading.Tasks;

namespace DNTFrameworkCore.Infrastructure.Tasks
{
    public interface IEndTask 
    {
        Task ExecuteAsync();
    }
}