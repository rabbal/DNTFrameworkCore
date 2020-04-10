using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskFilteredPagedRequestModel : FilteredPagedRequestModel
    {
        public TaskState? State { get; set; }
    }
}