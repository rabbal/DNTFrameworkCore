using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DNTFrameworkCore.EFCore.Context.Converters.Json
{
    public static class ModelBuilderExtensions
    {
        private static bool HasJsonAttribute(PropertyInfo propertyInfo)
        {
            return propertyInfo != null &&
                   propertyInfo.CustomAttributes.Any(a => a.AttributeType == typeof(JsonFieldAttribute));
        }

        /// <summary>
        /// Add fields marked with <see cref="JsonFieldAttribute"/> to be converted using <see cref="JsonValueConverter{T}"/>.
        /// </summary>
        /// <param name="modelBuilder">Model builder instance. Cannot be null.</param>
        /// <param name="skipConventionalEntities">
        ///   Skip trying to initialize properties for entity types found by EF conventions.
        ///   EF conventions treats complex fields as possible entity types. This can easily cause issues if we are cross referencing types utilizing
        ///   JsonAttribute while not registering them as actual entities in our db context.
        /// </param>
        /// <remarks>
        /// Adapted from https://www.tabsoverspaces.com/233708-using-value-converter-for-custom-encryption-of-field-on-entity-framework-core-2-1
        /// </remarks>
        public static void AddJsonFields(this ModelBuilder modelBuilder, bool skipConventionalEntities = true)
        {
            if (modelBuilder == null)
                throw new ArgumentNullException(nameof(modelBuilder));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var typeBase = typeof(TypeBase);
                if (skipConventionalEntities)
                {
                    var typeConfigurationSource = typeBase
                        .GetField("_configurationSource", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(entityType)?.ToString();
                    if (Enum.TryParse(typeConfigurationSource, out ConfigurationSource typeSource) &&
                        typeSource == ConfigurationSource.Convention) continue;
                }

                var ignoredMembers =
                    typeBase.GetField("_ignoredMembers", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.GetValue(entityType) as Dictionary<string, ConfigurationSource>;

                bool NotIgnored(PropertyInfo property) =>
                    property != null && !ignoredMembers.ContainsKey(property.Name) &&
                    property.CustomAttributes.All(a => a.AttributeType != typeof(NotMappedAttribute));

                foreach (var clrProperty in entityType.ClrType.GetProperties()
                    .Where(x => NotIgnored(x) && HasJsonAttribute(x)))
                {
                    var property = modelBuilder.Entity(entityType.ClrType)
                        .Property(clrProperty.PropertyType, clrProperty.Name);
                    var modelType = clrProperty.PropertyType;

                    var converterType = typeof(JsonValueConverter<>).MakeGenericType(modelType);
                    var converter = (ValueConverter) Activator.CreateInstance(converterType, new object[] {null});
                    property.Metadata.SetValueConverter(converter);

                    var valueComparer = typeof(JsonValueComparer<>).MakeGenericType(modelType);
                    property.Metadata.SetValueComparer(
                        (ValueComparer) Activator.CreateInstance(valueComparer, new object[0]));
                }
            }
        }
    }
}