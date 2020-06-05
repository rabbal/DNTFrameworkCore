using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DNTFrameworkCore.Querying
{
    public interface IFilteredPagedRequest : IPagedRequest
    {
        string Filtering { get; set; }
        IReadOnlyList<FilterExpression> ParsedFiltering { get; }
    }

    public class FilteredPagedRequest : PagedRequest, IFilteredPagedRequest
    {
        private const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";

        private static readonly Regex _regex = new Regex(EscapedCommaPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

        private List<FilterExpression> _parsedFiltering = new List<FilterExpression>();
        private string _filtering;

        public string Filtering
        {
            get => _filtering;
            set
            {
                _filtering = value;

                if (string.IsNullOrEmpty(value)) return;

                _parsedFiltering = ParseFiltering(_filtering);
            }
        }

        public IReadOnlyList<FilterExpression> ParsedFiltering => _parsedFiltering;

        protected virtual List<FilterExpression> ParseFiltering(string filtering)
        {
            return null;
        }
    }
}