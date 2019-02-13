using System;
using System.Reflection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Localization
{
    /// <summary>
    /// This class can be used to serialize <see cref="LocalizedString"/> to <see cref="string"/> during serialization.
    /// It does not work for deserialization.
    /// </summary>
    public class LocalizableStringToStringJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var localizedString = (LocalizedString) value;
            writer.WriteValue(localizedString);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(LocalizedString).GetTypeInfo().IsAssignableFrom(objectType);
        }
    }
}