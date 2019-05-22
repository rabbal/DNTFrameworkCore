using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskReadModel : ReadModel
    {
        public string Title { get; set; }
        public string Number { get; set; }
        public TaskState State { get; set; } = TaskState.Todo;
    }
}