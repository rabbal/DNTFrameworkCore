using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;

namespace DNTFrameworkCore.TestWebApp.Application.Tasks.Models
{
    public class TaskFilteredPagedQueryModel : FilteredPagedQueryModel
    {
        public TaskState? State { get; set; }
    }
}