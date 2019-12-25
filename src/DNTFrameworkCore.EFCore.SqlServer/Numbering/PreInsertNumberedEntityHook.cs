using System;
using System.Data;
using System.Globalization;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
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

        protected override void Hook(INumberedEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            if (!string.IsNullOrEmpty(entity.Number)) return;

            var option = _options.Value.NumberedEntityMap[entity.GetType()];

            bool retry;
            string number;
            do
            {
                number = NewNumber(entity, option, uow);
                retry = !IsUniqueNumber(entity, number, option, uow);
            } while (retry);

            uow.Entry(entity).Property(nameof(INumberedEntity.Number)).CurrentValue = number;
        }

        private static bool IsUniqueNumber(INumberedEntity entity, string number, NumberedEntityOption option, IUnitOfWork uow)
        {
            using (var command = uow.Connection.CreateCommand())
            {
                var parameterNames = option.FieldNames.Aggregate(string.Empty,
                    (current, fieldName) => current + $"AND [t0].[{fieldName}] = @{fieldName} ");

                var tableName = uow.Entry(entity).Metadata.GetTableName();
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

                foreach (var fieldName in option.FieldNames)
                {
                    var p = command.CreateParameter();

                    var value = uow.Entry(entity).Property(fieldName).CurrentValue;
                    switch (value)
                    {
                        case DateTimeOffset dateTimeOffset:
                            value = dateTimeOffset.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                            break;
                        case DateTime dateTime:
                            value = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                            break;
                    }

                    p.Value = value;
                    p.ParameterName = $"@{fieldName}";
                    p.DbType = SqlHelper.TypeMap[value.GetType()];

                    command.Parameters.Add(p);
                }

                command.Transaction = uow.Transaction.GetDbTransaction();

                var result = command.ExecuteScalar();

                return !Convert.ToBoolean(result);
            }
        }

        private static string NewNumber(INumberedEntity entity, NumberedEntityOption option, IUnitOfWork uow)
        {
            var key = CreateEntityKey(entity, option, uow);

            uow.AcquireDistributedLock(key);

            var number = option.Start.ToString();

            var numberedEntity = uow.Set<NumberedEntity>().AsNoTracking().FirstOrDefault(a => a.EntityName == key);
            if (numberedEntity == null)
            {
                uow.ExecuteSqlRawCommand(
                    "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextNumber]) VALUES(@p0,@p1)",
                    key,
                    option.Start + option.IncrementBy);
            }
            else
            {
                number = numberedEntity.NextNumber.ToString();
                uow.ExecuteSqlRawCommand(
                    "UPDATE [dbo].[NumberedEntity] SET [NextNumber] = @p0 WHERE [Id] = @p1 ",
                    numberedEntity.NextNumber + option.IncrementBy, numberedEntity.Id);
            }

            if (!string.IsNullOrEmpty(option.Prefix))
                number = option.Prefix + number;

            return number;
        }

        private static string CreateEntityKey(INumberedEntity entity, NumberedEntityOption option, IUnitOfWork uow)
        {
            var type = entity.GetType();

            var key = $"{type.FullName}";

            foreach (var fieldName in option.FieldNames)
            {
                var value = uow.Entry(entity).Property(fieldName).CurrentValue;
                switch (value)
                {
                    case DateTimeOffset dateTimeOffset:
                        value = dateTimeOffset.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                        break;
                    case DateTime dateTime:
                        value = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                        break;
                }

                key += $"_{fieldName}_{value}";
            }

            return key;
        }
    }
}