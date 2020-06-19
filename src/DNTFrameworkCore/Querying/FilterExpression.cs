using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Querying
{
    public class FilterExpression
    {
        /// <summary>
        /// Gets or sets the name of the filtered field (property).
        /// </summary>
        public string Field { get; set; }
        public string OperatorName { get; set; }
        public object Value { get; set; }

        /// <summary>
        /// Can be set to "or" or "and".
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// Gets or sets the statements in a group.
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