using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class PermissionModel : Model<int>
    {
        public string Name { get; set; }
    }
}