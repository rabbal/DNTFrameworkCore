using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class UserRoleModel : DetailModel<int>
    {
        public long RoleId { get; set; }
    }
}