using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleModalViewModel : RoleModel
    {
        public IReadOnlyList<LookupItem> PermissionList { get; set; }
    }
}