using System.Collections.Generic;

namespace DNTFrameworkCore.EFCore.Context
{
    public class EntityChangeContext
    {
        public IEnumerable<string> EntityNames { get; }

        public EntityChangeContext(IEnumerable<string> names)
        {
            EntityNames = names;
        }
    }
}