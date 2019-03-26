using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;

namespace DNTFrameworkCore.TestWebApp.Models.Roles
{
    public class RoleIndexViewModel
    {
        public PagedListModel<RoleReadModel, RoleFilteredPagedQueryModel> PagedList { get; set; }
        public IReadOnlyList<LookupItem> Permissions { get; set; }
    }
}