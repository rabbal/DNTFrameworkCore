using System.Collections.Generic;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class UserModel : MasterModel<long>
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public ICollection<long> Roles { get; set; } = new HashSet<long>();
        public ICollection<string> Permissions { get; set; } = new HashSet<string>();
        public ICollection<string> IgnoredPermissions { get; set; } = new HashSet<string>();
//         public bool ShouldMapSerial(User user) =>
//            IsNew() || !IsActive || !Password.IsEmpty() ||
//                Roles.Any(a => a.IsNew() || a.IsDeleted()) ||
//                IgnoredPermissions.Any(p => p.IsDeleted() || p.IsNew()) ||
//                Permissions.Any(p => p.IsDeleted() || p.IsNew());

        public bool ShouldMapPasswordHash() =>
            IsNew() || !Password.IsEmpty();
    }
}