using System;
using System.Collections.Generic;
using DNTFrameworkCore.Common;

namespace DNTFrameworkCore.Numbering
{
    public class NumberedEntityOption
    {
        public string FieldName { get; set; }
        public IEnumerable<string> Fields { get; set; } = new List<string>();
        public int Start { get; set; } = 1;
        public int IncrementBy { get; set; } = 1;
        public string Prefix { get; set; }

        public Func<NameValue<object>, object> Normalize { get; set; }
    }
}