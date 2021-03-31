using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Querying
{
    public interface IFilteredPagedRequest : IPagedRequest
    {
        string Filtering { get; set; }
        IEnumerable<FilterExpression> GetFilterExpressions();
        void SetFilterExpressions(IEnumerable<FilterExpression> expressions);
    }

    public class FilteredPagedRequest : PagedRequest, IFilteredPagedRequest
    {
        private const string EscapedCommaPattern = @"(?<!($|[^\\])(\\\\)*?\\),";

        private static readonly Regex _regex = new(EscapedCommaPattern,
            RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(2));

        private List<FilterExpression> _expressions = new();
        private string _filtering;

        public string Filtering
        {
            get => _filtering;
            set
            {
                _filtering = value;

                if (string.IsNullOrEmpty(value)) return;

                _expressions = ParseFiltering(_filtering);
            }
        }

        public IEnumerable<FilterExpression> GetFilterExpressions() => _expressions;
        public void SetFilterExpressions(IEnumerable<FilterExpression> expressions)
        {
            //TODO: maybe use add-range method
            _expressions = expressions?.ToList() ?? throw new ArgumentNullException(nameof(expressions));
        }

        //TODO: implement default parsing mechanism
        protected virtual List<FilterExpression> ParseFiltering(string filtering)
        {
            throw new NotImplementedException();
        }
        
        public void FilteringIfEmpty(string filtering)
        {
            Ensure.IsNotNullOrEmpty(filtering, nameof(filtering));
            if (!string.IsNullOrEmpty(Filtering)) return;
            
            Filtering = filtering;
        }
    }
}