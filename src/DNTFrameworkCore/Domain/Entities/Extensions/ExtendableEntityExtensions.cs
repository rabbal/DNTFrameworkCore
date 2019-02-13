using System.Collections.Generic;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.ReflectionToolkit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DNTFrameworkCore.Domain.Entities.Extensions
{
    public static class ExtendableEntityExtensions
    {
        public static T GetData<T>(this IExtendableEntity entity, string name, bool handleType = false)
        {
            return entity.GetData<T>(
                name,
                handleType
                    ? new JsonSerializer {TypeNameHandling = TypeNameHandling.All}
                    : JsonSerializer.CreateDefault()
            );
        }

        public static T GetData<T>(this IExtendableEntity entity, string name, JsonSerializer jsonSerializer)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(name, nameof(name));

            if (entity.ExtensionJson == null) return default(T);

            var json = JObject.Parse(entity.ExtensionJson);

            var prop = json[name];
            if (prop == null) return default(T);

            if (TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(T)))
                return prop.Value<T>();
            return (T) prop.ToObject(typeof(T), jsonSerializer ?? JsonSerializer.CreateDefault());
        }

        public static void SetData<T>(this IExtendableEntity entity, string name, T value,
            bool handleType = false)
        {
            entity.SetData(
                name,
                value,
                handleType
                    ? new JsonSerializer {TypeNameHandling = TypeNameHandling.All}
                    : JsonSerializer.CreateDefault()
            );
        }

        public static void SetData<T>(this IExtendableEntity entity, string name, T value,
            JsonSerializer jsonSerializer)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(name, nameof(name));

            if (jsonSerializer == null) jsonSerializer = JsonSerializer.CreateDefault();

            if (entity.ExtensionJson == null)
            {
                if (EqualityComparer<T>.Default.Equals(value, default(T))) return;

                entity.ExtensionJson = "{}";
            }

            var json = JObject.Parse(entity.ExtensionJson);

            if (value == null || EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                if (json[name] != null) json.Remove(name);
            }
            else if (TypeHelper.IsPrimitiveExtendedIncludingNullable(value.GetType()))
            {
                json[name] = new JValue(value);
            }
            else
            {
                json[name] = JToken.FromObject(value, jsonSerializer);
            }

            var data = json.ToString(Formatting.None);
            if (data == "{}") data = null;

            entity.ExtensionJson = data;
        }

        public static bool RemoveData(this IExtendableEntity entity, string name)
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            if (entity.ExtensionJson == null) return false;

            var json = JObject.Parse(entity.ExtensionJson);

            var token = json[name];
            if (token == null) return false;

            json.Remove(name);

            var data = json.ToString(Formatting.None);
            if (data == "{}") data = null;

            entity.ExtensionJson = data;

            return true;
        }

        //TODO: string[] GetExtendedPropertyNames(...)
    }
}