using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestAPI.Domain.Tasks;

namespace DNTFrameworkCore.TestAPI.Application.Tasks.Models
{
    public class TaskFilteredPagedQueryModel : FilteredPagedQueryModel
    {
        public TaskState? State { get; set; }
    }
}