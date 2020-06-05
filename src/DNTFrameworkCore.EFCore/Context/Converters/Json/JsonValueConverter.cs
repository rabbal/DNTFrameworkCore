using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DNTFrameworkCore.EFCore.Context.Converters.Json
{
    /// <summary>
    /// Converts complex field to/from JSON string.
    /// </summary>
    /// <typeparam name="T">Model field type.</typeparam>
    /// <remarks>See more: https://docs.microsoft.com/en-us/ef/core/modeling/value-conversions </remarks>
    public class JsonValueConverter<T> : ValueConverter<T, string> where T : class
    {
        public JsonValueConverter(ConverterMappingHints hints = default) :
            base(v => Serialize(v), v => Deserialize(v), hints)
        {
        }

        private static T Deserialize(string jsonString)
        {
            return string.IsNullOrWhiteSpace(jsonString) ? null : JsonSerializer.Deserialize<T>(jsonString);
        }

        private static string Serialize(T obj)
        {
            return obj == null ? null : JsonSerializer.Serialize(obj);
        }
    }
}