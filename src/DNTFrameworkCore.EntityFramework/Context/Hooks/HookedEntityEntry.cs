using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Hooks
{
    public class HookedEntityEntry
    {
        public object Entity { get; set; }
        public EntityState PreSaveState { get; set; }
    }
}