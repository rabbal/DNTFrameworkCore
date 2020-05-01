using System.Collections.Generic;

namespace DNTFrameworkCore.Metadata
{
    public class EntityMetadata
    {
        public string Name { get; set; }
        public IEnumerable<EntityViewMetadata> Views { get; set; }
    }
}