using System.Collections.Generic;
using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class RoleFilteredPagedRequestModel : FilteredPagedRequestModel
    {
        public IList<string> Permissions { get; set; }
    }
}