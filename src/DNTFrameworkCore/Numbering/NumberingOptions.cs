using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Numbering
{
    public class NumberingOptions
    {
        private readonly IDictionary<Type, IEnumerable<NumberedEntityOption>> _mappings =
            new Dictionary<Type, IEnumerable<NumberedEntityOption>>();

        public IEnumerable<NumberedEntityOption> this[Type type]
        {
            get => _mappings[type];
            set => _mappings[type] = value;
        }
    }
}