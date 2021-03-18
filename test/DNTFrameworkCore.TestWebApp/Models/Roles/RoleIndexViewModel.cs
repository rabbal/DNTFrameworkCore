using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleIndexViewModel : PagedResult<RoleReadModel>
    {
        public SelectList Permissions { get; set; }
    }
}