using System;
using System.Linq;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Numbering
{
    public static class NumberingHelper
    {
        public static Maybe<NumberedEntityOptionAttribute> FindNumberingOption(this Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(NumberedEntityOptionAttribute), false).SingleOrDefault();

            if (attribute != null) return attribute as NumberedEntityOptionAttribute;

            return Maybe<NumberedEntityOptionAttribute>.None;
        }
    }
}