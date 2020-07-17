using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Querying
{
    public class RuleExpression
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
    }
    
    public class FilterExpression
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public string Logic { get; set; }
        public IReadOnlyList<FilterExpression> Filters { get; set; }

        public IList<FilterExpression> ToFlatList()
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