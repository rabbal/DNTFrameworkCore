using DNTFrameworkCore.TestTenancy.Domain.Tasks;

namespace DNTFrameworkCore.TestTenancy.Application.Tasks.Models
{
    public class TaskFilteredPagedQueryModel : FilteredPagedQueryModel
    {
        public TaskState? State { get; set; }
    }
}