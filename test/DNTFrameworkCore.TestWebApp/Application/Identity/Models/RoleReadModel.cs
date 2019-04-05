using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class RoleReadModel : Model<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}