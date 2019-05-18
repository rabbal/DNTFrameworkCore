using System.Collections.Generic;

namespace DNTFrameworkCore.EntityFramework.Context
{
    public class SaveChangeContext
    {
        public IEnumerable<string> ChangedEntityNames { get; }

        public SaveChangeContext(IEnumerable<string> changedEntityNames)
        {
            ChangedEntityNames = changedEntityNames;
        }
    }
}