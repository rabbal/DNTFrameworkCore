using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DNTFrameworkCore.Querying
{
    public interface IPagedRequest
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string Sorting { get; set; }
        IReadOnlyList<SortExpression> ParsedSorting { get; }
    }

    public class PagedRequest : IPagedRequest
    {
        private const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";

        private static readonly Regex _regex = new Regex(EscapedCommaPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

        private List<SortExpression> _parsedSorting = new List<SortExpression>();
        private string _sorting;

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string Sorting
        {
            get => _sorting;
            set
            {
                _sorting = value;

                if (string.IsNullOrEmpty(value)) return;

                _parsedSorting = ParseSorting(_sorting);
            }
        }

        public IReadOnlyList<SortExpression> ParsedSorting => _parsedSorting.AsReadOnly();

        protected virtual List<SortExpression> ParseSorting(string sorting)
        {
            return _regex.Split(sorting).Where(sort => !string.IsNullOrWhiteSpace(sort))
                .Select(SortExpression.FromString).ToList();
        }

        public void SortingIfNullOrEmpty(string sorting)
        {
            if (!string.IsNullOrEmpty(Sorting)) return;
            Sorting = sorting;
        }
    }
}