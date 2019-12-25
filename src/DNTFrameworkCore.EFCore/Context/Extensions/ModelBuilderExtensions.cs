using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SpecifyDecimalPrecision(this ModelBuilder builder, int precision = 20, int scale = 6)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal)
                            || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType($"decimal({precision}, {scale})");
            }
        }

        public static void SpecifyDateTimeKind(this ModelBuilder builder)
        {
            // If you store a DateTime object to the DB with a DateTimeKind of either `Utc` or `Local`,
            // when you read that record back from the DB you'll get a DateTime object whose kind is `Unspecified`.
            // Here is a fix for it!
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => !v.HasValue ? v : (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime()),
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties()))
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}