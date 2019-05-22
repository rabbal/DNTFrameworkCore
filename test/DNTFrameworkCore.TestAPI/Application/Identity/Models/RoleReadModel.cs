using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestAPI.Application.Identity.Models
{
    public class RoleReadModel : ReadModel<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}