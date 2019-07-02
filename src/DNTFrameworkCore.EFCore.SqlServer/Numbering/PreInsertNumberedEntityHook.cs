using System;
using System.Globalization;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Numbering;
using DNTFrameworkCore.ReflectionToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EFCore.SqlServer.Numbering
{
    internal class PreInsertNumberedEntityHook : PreInsertHook<INumberedEntity>
    {
        private readonly IUnitOfWork _uow;
        private readonly IOptions<NumberingOptions> _options;
        private readonly ITenant _tenant;

        public PreInsertNumberedEntityHook(IUnitOfWork uow, IOptions<NumberingOptions> options, ITenant tenant)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        protected override void Hook(INumberedEntity entity, HookEntityMetadata metadata)
        {
            if (!string.IsNullOrEmpty(entity.Number)) return;

            bool retry;
            string nextNumber;

            do
            {
                nextNumber = BuildNumber(entity);
                retry = !IsUniqueNumber(entity, nextNumber);
            } while (retry);

            _uow.Entry(entity).Property(nameof(INumberedEntity.Number)).CurrentValue = nextNumber;
        }
        
        //Todo: Refactor and Improve ResetField Scenarios 
        private bool IsUniqueNumber(INumberedEntity entity, string nextNumber)
        {
            var option = _options.Value.NumberedEntityMap[entity.GetType()];
            var resetClause = string.Empty;
            if (!string.IsNullOrEmpty(option.ResetFieldName))
            {
                var value = new PropertyReflector().GetValue(entity, option.ResetFieldName).ToString();
                if (DateTimeOffset.TryParse(value, out var dateTime))
                {
                    value = dateTime.ToString(CultureInfo.InvariantCulture);
                }

                resetClause += $"AND [{option.ResetFieldName}] = '{value}'";
            }

            using (var command = _uow.Connection.CreateCommand())
            {
                command.CommandText = $@"SELECT
                    (CASE
                WHEN EXISTS(
                    SELECT NULL AS [EMPTY]
                        FROM [{_uow.Entry(entity).Metadata.Relational().TableName}] AS [t0]
                        WHERE [t0].[Number] = '{nextNumber}' {resetClause}
                ) THEN 0
                ELSE 1
                END) [Value]";

                command.Transaction = _uow.Transaction.GetDbTransaction();

                var result = command.ExecuteScalar();

                return Convert.ToBoolean(result);
            }
        }

        private string BuildNumber(INumberedEntity entity)
        {
            var entityType = entity.GetType();
            var option = _options.Value.NumberedEntityMap[entityType];

            var entityName = $"{entityType.FullName}";

            if (!string.IsNullOrEmpty(option.ResetFieldName))
            {
                var value = new PropertyReflector().GetValue(entity, option.ResetFieldName).ToString();
                if (DateTimeOffset.TryParse(value, out var dateTime))
                {
                    value = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                }

                entityName += $"_{option.ResetFieldName}_{value}";
            }

            var lockKey = entityName;

            if (_tenant.HasValue)
            {
                lockKey = $"Tenant_{_tenant.Value.Name}_{lockKey}";
            }

            _uow.AcquireDistributedLock(lockKey);

            var nextNumber = option.Start.ToString();

            var numberedEntity = _uow.Set<NumberedEntity>().AsNoTracking()
                .FirstOrDefault(a => a.EntityName == entityName);
            if (numberedEntity == null)
            {
                if (_tenant.HasValue)
                {
                    _uow.ExecuteSqlCommand(
                        "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextNumber], [TenantId]) VALUES(@p0,@p1,@p2)",
                        entityName,
                        option.Start + option.IncrementBy, _tenant.Value.Id);
                }
                else
                {
                    _uow.ExecuteSqlCommand(
                        "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextNumber]) VALUES(@p0,@p1)",
                        entityName,
                        option.Start + option.IncrementBy);
                }
            }
            else
            {
                nextNumber = numberedEntity.NextNumber.ToString();
                _uow.ExecuteSqlCommand(
                    "UPDATE [dbo].[NumberedEntity] SET [NextNumber] = @p0 WHERE [Id] = @p1 ",
                    numberedEntity.NextNumber + option.IncrementBy, numberedEntity.Id);
            }

            if (!string.IsNullOrEmpty(option.Prefix))
                nextNumber = option.Prefix + nextNumber;

            return nextNumber;
        }
    }
}