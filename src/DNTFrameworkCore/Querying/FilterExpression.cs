using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Querying
{
    public class FilterExpression
    {
        /// <summary>
        /// Gets or sets the name of the sorted field (property). Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the filtering operator. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// Gets or sets the filtering value. Set to <c>null</c> if the <c>Filters</c> property is set.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the filtering logic. Can be set to "or" or "and". Set to <c>null</c> unless <c>Filters</c> is set.
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// Gets or sets the child filter expressions. Set to <c>null</c> if there are no child expressions.
        /// </summary>
        public IReadOnlyList<FilterExpression> Filters { get; set; }

        public IList<FilterExpression> ToFlattenList()
        {
            var output = new List<FilterExpression>();

            Collect(output);

            return output;
        }

        private void Collect(ICollection<FilterExpression> output)
        {
            if (Filters != null && Filters.Any())
            {
                foreach (var filter in Filters)
                {
                    output.Add(filter);

                    filter.Collect(output);
                }
            }
            else
            {
                output.Add(this);
            }
        }
    }
}