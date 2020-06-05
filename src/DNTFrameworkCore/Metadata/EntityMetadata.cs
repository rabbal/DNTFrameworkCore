using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Metadata
{
    public class EntityMetadata
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<EntityViewMetadata> Views { get; set; }
        public Type ServiceType { get; set; }
    }
}