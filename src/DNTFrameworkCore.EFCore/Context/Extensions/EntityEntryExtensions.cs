using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    public static class EntityEntryExtensions
    {
        public static Dictionary<string, object> ToDictionary(this EntityEntry entry,
            Func<PropertyEntry, bool> predicate)
        {
            return entry.Properties.Where(predicate)
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
        }
    }
}