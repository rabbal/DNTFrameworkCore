using System.Collections.Generic;
using DNTFrameworkCore.Helpers;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// List of selector functions to select classes/interfaces to be audited.
    /// </summary>
    public interface IAuditingSelectorList : IList<NamedTypeSelector>
    {
        /// <summary>
        /// Removes a selector by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool RemoveByName(string name);
    }

    internal class AuditingSelectorList : List<NamedTypeSelector>, IAuditingSelectorList
    {
        public bool RemoveByName(string name)
        {
            return RemoveAll(s => s.Name == name) > 0;
        }
    }
}