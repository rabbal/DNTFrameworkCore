using System.Collections.Generic;
using DNTFrameworkCore.Common;

namespace DNTFrameworkCore.TestWebApp.Models.Users
{
    public class UserIndexViewModel
    {
        public IReadOnlyList<LookupItem<long>> Roles { get; set; }
    }
}