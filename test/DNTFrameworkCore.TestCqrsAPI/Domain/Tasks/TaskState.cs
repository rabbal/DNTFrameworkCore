using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Tasks
{
    public class TaskState : Enumeration
    {
        public static TaskState Todo = new TaskState(1, "Todo");
        public static TaskState Doing = new TaskState(2, "Doing");
        public static TaskState Done = new TaskState(3, "Done");

        public TaskState(int id, string name) : base(id, name)
        {
        }
    }
}
