using System.Collections.Generic;
using DNTFrameworkCore.Common;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleModalViewModel : RoleModel
    {
        public IReadOnlyList<LookupItem> PermissionList { get; set; }
    }
}