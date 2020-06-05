using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
namespace DNTFrameworkCore.EFCore.Context.Converters.Json
{
    /// <summary>
    /// Compares two objects.
    /// Required to make EF Core change tracking work for complex value converted objects.
    /// </summary>
    /// <remarks>
    /// For objects that implement <see cref="ICloneable"/> and <see cref="IEquatable{T}"/>,
    /// those implementations will be used for cloning and equality.
    /// For plain objects, fall back to deep equality comparison using JSON serialization
    /// (safe, but inefficient).
    /// </remarks>
    internal class JsonValueComparer<T> : ValueComparer<T>
    {
        private static string Json(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        private static T TakeSnapshot(T instance)
        {
            if (instance is ICloneable cloneable)
                return (T) cloneable.Clone();

            return (T) JsonSerializer.Deserialize(Json(instance), typeof(T));
        }

        private static int HashCode(T instance)
        {
            return instance is IEquatable<T> ? instance.GetHashCode() : Json(instance).GetHashCode();
        }

        private static bool AreEqual(T left, T right)
        {
            if (left is IEquatable<T> equatable)
                return equatable.Equals(right);

            return Json(left).Equals(Json(right));
        }

        public JsonValueComparer() : base(
            (t1, t2) => AreEqual(t1, t2),
            t => HashCode(t),
            t => TakeSnapshot(t))
        {
        }
    }
}