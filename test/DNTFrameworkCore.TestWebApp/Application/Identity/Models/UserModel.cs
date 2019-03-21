using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class UserModel : MasterModel
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserRoleModel> Roles { get; set; } = new HashSet<UserRoleModel>();
        public ICollection<PermissionModel> Permissions { get; set; } = new HashSet<PermissionModel>();
        public ICollection<PermissionModel> IgnoredPermissions { get; set; } = new HashSet<PermissionModel>();
    }
}