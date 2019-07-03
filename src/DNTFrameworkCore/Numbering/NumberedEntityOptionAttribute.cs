using System;

namespace DNTFrameworkCore.Numbering
{
    public class NumberedEntityOptionAttribute : Attribute
    {
        public string Prefix { get; set; }
        public int Start { get; set; } = 1;
        public int IncrementBy { get; set; } = 1;
        public string ResetFieldName { get; set; }
    }
}