using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Numbering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EFCore.SqlServer.Numbering
{
    internal class PreInsertNumberedEntityHook : PreInsertHook<INumberedEntity>
    {
        private readonly IOptions<NumberingOptions> _options;

        public PreInsertNumberedEntityHook(IOptions<NumberingOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public override string Name => HookNames.Numbering;

        protected override void Hook(INumberedEntity entity, HookEntityMetadata metadata, IDbContext dbContext)
        {
            var options = _options.Value[entity.GetType()].ToList();
            foreach (var option in options)
            {
                if (!string.IsNullOrEmpty(dbContext.PropertyValue<string>(entity, option.FieldName))) return;

                bool retry;
                string number;
                do
                {
                    number = NewNumber(entity, option, dbContext);
                    retry = !IsUniqueNumber(entity, number, option.Fields, dbContext);
                } while (retry);

                dbContext.Entry(entity).Property(option.FieldName).CurrentValue = number;
            }
        }

        private static bool IsUniqueNumber(INumberedEntity entity, string number, IEnumerable<string> fields,
            IDbContext dbContext)
        {
            fields = fields.ToList();
            using var command = dbContext.Connection.CreateCommand();
            var parameterNames = fields.Aggregate(string.Empty,
                (current, fieldName) => $"{current} AND [t0].[{fieldName}] = @{fieldName} ");

            var tableName = dbContext.Entry(entity).Metadata.GetTableName();
            command.CommandText = $@"SELECT
                    (CASE
                WHEN EXISTS(
                    SELECT NULL AS [EMPTY]
                        FROM [{tableName}] AS [t0]
                        WHERE [t0].[Number] = @Number {parameterNames}
                ) THEN 1
                ELSE 0
                END) [Value]";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@Number";
            parameter.Value = number;
            parameter.DbType = DbType.String;
            command.Parameters.Add(parameter);

            foreach (var field in fields)
            {
                var p = command.CreateParameter();

                var value = dbContext.Entry(entity).Property(field).CurrentValue;

                p.Value = value;
                p.ParameterName = $"@{field}";
                p.DbType = SqlHelper.TypeMapping[value.GetType()];

                command.Parameters.Add(p);
            }

            command.Transaction = dbContext.Transaction;

            var result = command.ExecuteScalar();

            return !Convert.ToBoolean(result);
        }

        private static string NewNumber(INumberedEntity entity, NumberedEntityOption option, IDbContext dbContext)
        {
            var key = CreateEntityKey(entity, option.Fields, dbContext);

            dbContext.AcquireDistributedLock(key);

            var number = option.Start.ToString(CultureInfo.InvariantCulture);

            var numberedEntity = dbContext.Set<NumberedEntity>().AsNoTracking().FirstOrDefault(a => a.EntityName == key);
            if (numberedEntity == null)
            {
                dbContext.ExecuteSqlRawCommand(
                    "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextValue]) VALUES(@p0,@p1)",
                    key,
                    option.Start + option.IncrementBy);
            }
            else
            {
                number = numberedEntity.NextValue.ToString(CultureInfo.InvariantCulture);
                dbContext.ExecuteSqlRawCommand(
                    "UPDATE [dbo].[NumberedEntity] SET [NextValue] = @p0 WHERE [Id] = @p1 ",
                    numberedEntity.NextValue + option.IncrementBy, numberedEntity.Id);
            }

            if (!string.IsNullOrEmpty(option.Prefix))
                number = option.Prefix + number;

            return number;
        }

        private static string CreateEntityKey(INumberedEntity entity, IEnumerable<string> fields, IDbContext dbContext)
        {
            var type = entity.GetType();

            var key = type.FullName;

            foreach (var field in fields)
            {
                var value = dbContext.Entry(entity).Property(field).CurrentValue;
                value = NormalizeValue(value);

                key += $"_{field}_{value}";
            }

            return key;
        }

        private static object NormalizeValue(object value)
        {
            switch (value)
            {
                case DateTimeOffset dateTimeOffset:
                    value = dateTimeOffset.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    break;
                case DateTime dateTime:
                    value = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                    break;
            }

            return value;
        }
    }
}