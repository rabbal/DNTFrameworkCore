using System;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Infrastructure.Tasks
{
    public interface IErrorTask
    {
        Task ExecuteAsync(Exception exception);
    }
}