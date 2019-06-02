using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Extensions;
namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class UserModel : MasterModel<long>
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<UserRoleModel> Roles { get; set; } = new HashSet<UserRoleModel>();
        public ICollection<PermissionModel> Permissions { get; set; } = new HashSet<PermissionModel>();
        public ICollection<PermissionModel> IgnoredPermissions { get; set; } = new HashSet<PermissionModel>();
         public bool ShouldMapSerialNumber() =>
            IsNew() || !IsActive || !Password.IsEmpty() ||
                Roles.Any(a => a.IsNew() || a.IsDeleted()) ||
                IgnoredPermissions.Any(p => p.IsDeleted() || p.IsNew()) ||
                Permissions.Any(p => p.IsDeleted() || p.IsNew());

        public bool ShouldMapPasswordHash() =>
            IsNew() || !Password.IsEmpty();
    }
}