using System.Collections.Generic;
using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.TestWebApp.Application.Identity.Models
{
    public class RoleFilteredPagedRequest : FilteredPagedRequest
    {
        public IList<string> Permissions { get; set; }
    }
}