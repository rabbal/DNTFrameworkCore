using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleIndexViewModel : PagedQueryResult<RoleReadModel>
    {
        public IReadOnlyList<LookupItem> Permissions { get; set; }
    }
}