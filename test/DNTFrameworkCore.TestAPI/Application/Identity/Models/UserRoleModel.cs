using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestAPI.Application.Identity.Models
{
    public class UserRoleModel : Model<int>
    {
        public long RoleId { get; set; }
    }
}