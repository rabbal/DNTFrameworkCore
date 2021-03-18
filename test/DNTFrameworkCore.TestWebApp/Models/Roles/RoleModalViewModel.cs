using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleModalViewModel : RoleModel
    {
        public SelectList Permissions { get; set; }
    }
}