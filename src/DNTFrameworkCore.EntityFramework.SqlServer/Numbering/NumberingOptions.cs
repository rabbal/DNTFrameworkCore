using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.EntityFramework.SqlServer.Numbering
{
    public class NumberingOptions
    {
        public IDictionary<Type, NumberedEntityOption> NumberedEntityMap { get; } =
            new Dictionary<Type, NumberedEntityOption>();
    }
}