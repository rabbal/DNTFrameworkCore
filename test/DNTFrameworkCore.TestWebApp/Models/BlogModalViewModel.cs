using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;

namespace DNTFrameworkCore.TestWebApp.Models
{
    public class BlogViewModel : BlogModel
    {
        public bool IsAdmin { get; set; }
    }
}
