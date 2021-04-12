using System.Collections.Generic;
using System.Linq.Expressions;

namespace DNTFrameworkCore.Querying
{
    public interface IFilterExpression
    {
        Expression CreateLinqExpression();
    }

    public class CompositeFilterExpression : IFilterExpression
    {
        public string Logic { get; set; }
        public IReadOnlyList<IFilterExpression> Filters { get; set; }

        public Expression CreateLinqExpression()
        {
            throw new System.NotImplementedException();
        }

        public CompositeFilterExpression Add(IFilterExpression filterExpression)
        {
            return this;
        }
    }

    public class FilterExpression : IFilterExpression
    {
        public string Field { get; set; }
        public FilteringOperator Operator { get; set; }
        public object Value { get; set; }

        public Expression CreateLinqExpression()
        {
            throw new System.NotImplementedException();
        }

        // /// <summary>
        // /// Converts the filter expression to a predicate suitable for Dynamic Linq e.g. "Field1 = @1 and Field2.Contains(@2)"
        // /// </summary>
        // private static string ToExpression(FilterExpression filterExpression,
        //     Func<FilterExpression, int> indexFactory)
        // {
        //     if (filterExpression.Filters != null && filterExpression.Filters.Any())
        //     {
        //         return "(" + string.Join(" " + filterExpression.Logic + " ",
        //                    filterExpression.Filters.Select(filter => filter.ToExpression(indexFactory)).ToArray()) +
        //                ")";
        //     }

        //     var index = indexFactory(filterExpression);

        //     var comparison = _operators[filterExpression.Operator];

        //     if (filterExpression.Operator == "doesnotcontain")
        //     {
        //         return $"!{filterExpression.Field}.{comparison}(@{index})";
        //     }

        //     if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
        //     {
        //         return $"{filterExpression.Field}.{comparison}(@{index})";
        //     }

        //     return $"{filterExpression.Field} {comparison} @{index}";
        // }

        private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
        {
            {QueryingOperator.Equal, "="},
            {QueryingOperator.NotEqual, "!="},
            {QueryingOperator.LessThan, "<"},
            {QueryingOperator.LessThanOrEqual, "<="},
            {QueryingOperator.GreaterThan, ">"},
            {QueryingOperator.GreaterThanOrEqual, ">="},
            {QueryingOperator.StartsWith, "StartsWith"},
            {QueryingOperator.EndsWith, "EndsWith"},
            {QueryingOperator.Contains, "Contains"},
            {QueryingOperator.DoesNotContain, "Contains"},
            {QueryingOperator.IsNull, "="},
            {QueryingOperator.IsNotNull, "!="},
            {QueryingOperator.IsEmpty, "="},
            {QueryingOperator.IsNotEmpty, "!="},
            {QueryingOperator.IsNullOrEmpty, ""},
            {QueryingOperator.IsNotNullOrEmpty, "!"}
        };
    }
}