using DNTFrameworkCore.TestTenancy.Domain.Tasks;

namespace DNTFrameworkCore.TestTenancy.Application.Tasks.Models
{
    public class TaskReadModel : ReadModel
    {
        public string Title { get; set; }
        public string Number { get; set; }
        public TaskState State { get; set; } = TaskState.Todo;
    }
}