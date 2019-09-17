using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DNTFrameworkCore.EFCore.Context
{
    public class EntityChangeContext
    {
        public IEnumerable<string> EntityNames { get; }
        public IEnumerable<EntityEntry> EntityEntries { get; }

        public EntityChangeContext(IEnumerable<string> names, IEnumerable<EntityEntry> entries)
        {
            EntityNames = names;
            EntityEntries = entries;
        }
    }
}