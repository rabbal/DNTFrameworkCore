using System.Collections.Generic;

namespace DNTFrameworkCore.Numbering
{
    public class NumberedEntityOption
    {
        public int Start { get; set; } = 1;
        public int IncrementBy { get; set; } = 1;
        public string Prefix { get; set; }

        /// <summary>
        /// Name of fields that used for reset next-value
        /// </summary>
        public IEnumerable<string> Fields { get; set; } = new List<string>();
    }
}