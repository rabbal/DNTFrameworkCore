using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestAPI.Application.Identity.Models
{
    public class RoleModel : MasterModel<long>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<PermissionModel> Permissions { get; set; } = new HashSet<PermissionModel>();
    }
}