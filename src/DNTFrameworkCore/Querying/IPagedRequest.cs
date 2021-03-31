using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Querying
{
    public interface IPagedRequest
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string Sorting { get; set; }
        IEnumerable<SortExpression> GetSortExpressions();
        void SetSortExpressions(IEnumerable<SortExpression> expressions);
    }

    public class PagedRequest : IPagedRequest
    {
        private const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";

        private static readonly Regex _regex = new(EscapedCommaPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

        private List<SortExpression> _expressions = new();
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

                _expressions = ParseSorting(_sorting);
            }
        }

        public IEnumerable<SortExpression> GetSortExpressions() => _expressions;

        public void SetSortExpressions(IEnumerable<SortExpression> expressions)
        {
            //TODO: maybe use add-range method
            _expressions = expressions?.ToList() ?? throw new ArgumentNullException(nameof(expressions));
        }

        protected virtual List<SortExpression> ParseSorting(string sorting)
        {
            return _regex.Split(sorting).Where(sort => !string.IsNullOrWhiteSpace(sort))
                .Select(SortExpression.FromString).ToList();
        }

        public void SortingIfEmpty(string sorting)
        {
            Ensure.IsNotNullOrEmpty(sorting, nameof(sorting));
            if (!string.IsNullOrEmpty(Sorting)) return;

            Sorting = sorting;
        }
    }
}