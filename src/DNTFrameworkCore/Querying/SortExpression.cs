using System;
using System.Text.RegularExpressions;

namespace DNTFrameworkCore.Querying
{
    public class SortExpression
    {
        private const string ExpressionPattern =
            @"(?<direction>desc|asc)\((?<field>[a-zA-Z_][a-zA-Z0-9_]*)\)|(?<field>[a-zA-Z_][a-zA-Z0-9_]*)[_.:\s]{1}?(?=(?<direction>desc|asc))|(?<=(?<direction>[-+]?))(?<field>[a-zA-Z_][a-zA-Z0-9_]*)";

        private static readonly Regex _regex = new(ExpressionPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase,
            TimeSpan.FromSeconds(2));

        public SortExpression(string field, bool descending)
        {
            if (string.IsNullOrEmpty(field)) throw new ArgumentNullException(nameof(field));

            Field = field;
            Descending = descending;
        }

        /// <summary>
        /// Gets or sets the name of the sorted field (property).
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Gets or sets the sort direction. Should be "true" for descending and "false" for "ascending".
        /// </summary>
        public bool Descending { get; }

        public override bool Equals(object obj)
        {
            if (obj is not SortExpression sort) return false;

            if (ReferenceEquals(this, obj)) return true;

            return Field.Equals(sort.Field) && Descending == sort.Descending;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Field != null ? Field.GetHashCode() : 0) * 397) ^ Descending.GetHashCode();
            }
        }

        public override string ToString()
        {
            return $"{Field} {(Descending ? "desc" : "asc")}";
        }

        public static SortExpression FromString(string sort)
        {
            if (string.IsNullOrEmpty(sort)) throw new ArgumentNullException(nameof(sort));

            var result = _regex.Match(sort);
            if (!result.Success)
                throw new ArgumentException(
                    "Invalid sort expression pattern! supported patterns: field.desc|field_desc|field:desc|field desc|-field|desc(field)");

            var field = result.Groups["field"].Value;
            var direction = result.Groups["direction"].Value;

            return new SortExpression(field, direction == "-" || direction == "desc");
        }
    }
}