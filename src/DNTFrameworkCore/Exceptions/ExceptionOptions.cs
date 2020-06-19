using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace DNTFrameworkCore.Exceptions
{
    public class ExceptionOptions
    {
        private static readonly Regex _regex = new Regex(@"\W", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public List<ExceptionMapItem> Mappings { get; } = new List<ExceptionMapItem>();
        [Required] public string DbException { get; set; }

        [Required] public string DbConcurrencyException { get; set; }

        [Required] public string InternalServerIssue { get; set; }

        public bool TryFindMapping(DbException dbException, out ExceptionMapItem mapping)
        {
            var words = new HashSet<string>(_regex.Split(dbException.ToStringFormat()));

            mapping = Mappings.FirstOrDefault(a => a.Keywords.IsProperSubsetOf(words));
            
            return mapping != null;
        }
    }

    public class ExceptionMapItem
    {
        public ISet<string> Keywords { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        [Required] public string Message { get; set; }
        public string MemberName { get; set; }
    }
}