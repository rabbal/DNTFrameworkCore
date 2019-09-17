using System.Collections.Generic;
using DNTFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    public class HookEntityMetadata
    {
        private readonly Dictionary<string, object> _properties;

        public HookEntityMetadata(EntityEntry entry)
        {
            Entry = entry;

            _properties = new Dictionary<string, object>();

            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary) continue;
                
                var propertyName = property.Metadata.Name;
                _properties[propertyName] = property.CurrentValue;
            }
        }

        public EntityEntry Entry { get; }
        public IReadOnlyDictionary<string, object> Properties => _properties.AsReadOnly();
    }
}