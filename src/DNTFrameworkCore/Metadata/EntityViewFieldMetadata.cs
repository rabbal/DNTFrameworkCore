using System;

namespace DNTFrameworkCore.Metadata
{
    public class EntityViewFieldMetadata
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool Sorting { get; set; }
        public bool Filtering { get; set; }
        public EntityViewFieldMetadata View { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EntityViewFieldAttribute : Attribute
    {
        public bool Sorting { get; set; }
        public bool Filtering { get; set; }
    }
}