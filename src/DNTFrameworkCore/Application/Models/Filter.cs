using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Application.Models
{
    public class Filter
    {
        private static readonly IDictionary<string, string> Operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };

        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }

        /// <summary>
        ///     Gets or sets the filtering logic. Can be set to "or" or "and". Set to <c>null</c> unless <c>Filters</c> is set.
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        ///     Gets or sets the child filter expressions. Set to <c>null</c> if there are no child expressions.
        /// </summary>
        public IEnumerable<Filter> Filters { get; set; }

        /// <summary>
        ///     Get a flattened list of all child filter expressions.
        /// </summary>
        public IList<Filter> List()
        {
            var filters = new List<Filter>();

            Collect(filters);

            return filters;
        }

        private void Collect(ICollection<Filter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                foreach (var filter in Filters)
                {
                    filters.Add(filter);

                    filter.Collect(filters);
                }
            }
            else
            {
                filters.Add(this);
            }
        }

        /// <summary>
        ///     Converts the filter expression to a predicate suitable for Dynamic Linq e.g. "Field1 = @1 and Field2.Contains(@2)"
        /// </summary>
        /// <param name="filters">A list of flattened filters.</param>
        public string ToExpression(IList<Filter> filters)
        {
            if (Filters != null && Filters.Any())
            {
                return "(" + string.Join(" " + Logic + " ", Filters.Select(filter => filter.ToExpression(filters)).ToArray()) + ")";
            }

            var index = filters.IndexOf(this);

            var comparison = Operators[Operator];

            if (Operator == "doesnotcontain")
            {
                return $"!{Field}.{comparison}(@{index})";
            }

            if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
            {
                return $"{Field}.{comparison}(@{index})";
            }

            return $"{Field} {comparison} @{index}";
        }
    }
}