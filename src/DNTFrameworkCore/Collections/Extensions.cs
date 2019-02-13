using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Collections
{
    public static class Extensions
    {
        public static void AddRange<T>(this ICollection<T> initial, IEnumerable<T> other)
        {
            if (other == null)
                return;

            if (initial is List<T> list)
            {
                list.AddRange(other);
                return;
            }

            foreach (var item in other)
            {
                initial.Add(item);
            }
        }

        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || !source.Any();
        }


        public static bool EqualsAll<T>(this IList<T> a, IList<T> b)
        {
            if (a == null || b == null)
                return a == null && b == null;

            if (a.Count != b.Count)
                return false;

            var comparer = EqualityComparer<T>.Default;

            return !a.Where((t, i) => !comparer.Equals(t, b[i])).Any();
        }
    }
}