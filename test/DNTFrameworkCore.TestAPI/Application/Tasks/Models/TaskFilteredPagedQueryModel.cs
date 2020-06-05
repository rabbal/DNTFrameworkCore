using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskFilteredPagedRequestModel : FilteredPagedRequest
    {
        public TaskState? State { get; set; }
    }
}