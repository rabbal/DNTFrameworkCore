using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleIndexViewModel : PagedResult<RoleReadModel>
    {
        public IReadOnlyList<LookupItem> Permissions { get; set; }
    }
}