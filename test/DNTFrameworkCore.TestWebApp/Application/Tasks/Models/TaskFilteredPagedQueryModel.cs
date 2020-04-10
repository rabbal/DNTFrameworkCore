using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks.Models
{
    public class TaskFilteredPagedRequest : FilteredPagedRequestModel
    {
        public TaskState? State { get; set; }
    }
}