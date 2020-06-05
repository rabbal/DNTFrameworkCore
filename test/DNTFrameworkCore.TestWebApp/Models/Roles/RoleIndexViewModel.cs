using System.Collections.Generic;
using DNTFrameworkCore.Common;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleIndexViewModel : PagedResult<RoleReadModel>
    {
        public IReadOnlyList<LookupItem> Permissions { get; set; }
    }
}