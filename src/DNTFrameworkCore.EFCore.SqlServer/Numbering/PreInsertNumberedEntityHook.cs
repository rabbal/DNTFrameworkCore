using System;
using System.Data;
using System.Globalization;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Numbering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EFCore.SqlServer.Numbering
{
    internal class PreInsertNumberedEntityHook : PreInsertHook<INumberedEntity>
    {
        private readonly IUnitOfWork _uow;
        private readonly ITenant _tenant;
        private readonly IOptions<NumberingOptions> _options;

        public PreInsertNumberedEntityHook(IUnitOfWork uow, ITenant tenant, IOptions<NumberingOptions> options)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void Hook(INumberedEntity entity, HookEntityMetadata metadata)
        {
            if (!string.IsNullOrEmpty(entity.Number)) return;

            var option = _options.Value.NumberedEntityMap[entity.GetType()];

            bool retry;
            string number;
            do
            {
                number = BuildNumber(entity, option);
                retry = !IsUniqueNumber(entity, number, option);
            } while (retry);

            _uow.Entry(entity).Property(nameof(INumberedEntity.Number)).CurrentValue = number;
        }

        private bool IsUniqueNumber(INumberedEntity entity, string number, NumberedEntityOption option)
        {
            using (var command = _uow.Connection.CreateCommand())
            {
                var parameterNames = option.FieldNames.Aggregate(string.Empty,
                    (current, fieldName) => current + $"AND [t0].[{fieldName}] = @{fieldName} ");

                var tableName = _uow.Entry(entity).Metadata.Relational().TableName;
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

                    var value = _uow.Entry(entity).Property(fieldName).CurrentValue;
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

                command.Transaction = _uow.Transaction.GetDbTransaction();

                var result = command.ExecuteScalar();

                return !Convert.ToBoolean(result);
            }
        }

        private string BuildNumber(INumberedEntity entity, NumberedEntityOption option)
        {
            var key = BuildEntityKey(entity, option);

            _uow.AcquireDistributedLock(key);

            var number = option.Start.ToString();

            var numberedEntity = _uow.Set<NumberedEntity>().AsNoTracking().FirstOrDefault(a => a.EntityName == key);
            if (numberedEntity == null)
            {
                _uow.ExecuteSqlCommand(
                    "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextNumber]) VALUES(@p0,@p1)",
                    key,
                    option.Start + option.IncrementBy);
            }
            else
            {
                number = numberedEntity.NextNumber.ToString();
                _uow.ExecuteSqlCommand(
                    "UPDATE [dbo].[NumberedEntity] SET [NextNumber] = @p0 WHERE [Id] = @p1 ",
                    numberedEntity.NextNumber + option.IncrementBy, numberedEntity.Id);
            }

            if (!string.IsNullOrEmpty(option.Prefix))
                number = option.Prefix + number;

            return number;
        }

        private string BuildEntityKey(INumberedEntity entity, NumberedEntityOption option)
        {
            var type = entity.GetType();

            var key = $"{type.FullName}";

            foreach (var fieldName in option.FieldNames)
            {
                var value = _uow.Entry(entity).Property(fieldName).CurrentValue;
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

            if (typeof(ITenantEntity).IsAssignableFrom(type))
            {
                key = $"Tenant_{_tenant.Value.Name}_{key}";
            }

            return key;
        }
    }
}