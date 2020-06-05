using System;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskReadModel : ReadModel
    {
        public string Title { get; set; }
        public string Number { get; set; }
        public TaskState State { get; set; } = TaskState.Todo;
        public DateTime LocalDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? NullableDateTime { get; set; }
    }
}