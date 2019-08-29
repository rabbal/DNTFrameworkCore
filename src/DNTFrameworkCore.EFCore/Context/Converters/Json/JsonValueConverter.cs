using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

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

        private static T Deserialize(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? null : JsonConvert.DeserializeObject<T>(json);
        }

        private static string Serialize(T obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj);
        }
    }
}