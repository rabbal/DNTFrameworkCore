using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskFilteredPagedRequest : FilteredPagedRequest
    {
        public TaskState? State { get; set; }
    }
}