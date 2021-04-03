using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DNTFrameworkCore.Extensibility
{
    public static class UniqueIdExtensions<T> where T : class
    {
        private static readonly ConditionalWeakTable<T, string> _ids = new();

        // A static field is shared across all instances of the `same` type or T here.
        // This behavior is useful to produce unique auto increment Id's per each different object reference.
        private static int _uniqueId;

        public static string UniqueId(T instance)
        {
            return _ids.GetValue(instance,
                o => Interlocked.Increment(ref _uniqueId).ToString(CultureInfo.InvariantCulture));
        }

        public static string UniqueId(T instance, string key)
        {
            return _ids.GetValue(instance, o => key);
        }
    }
}