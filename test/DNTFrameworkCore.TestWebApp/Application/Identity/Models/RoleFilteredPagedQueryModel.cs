using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class RoleFilteredPagedQueryModel : FilteredPagedQueryModel
    {
        public IList<string> Permissions { get; set; }
    }
}