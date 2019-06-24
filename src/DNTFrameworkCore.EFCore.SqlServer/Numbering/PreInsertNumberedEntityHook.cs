using System;
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
                nextNumber = GenerateNumber(entity);
                var exists = CheckDuplicateNumber(entity, nextNumber);
                retry = exists;
            } while (retry);

            _uow.Entry(entity).Property(nameof(INumberedEntity.Number)).CurrentValue = nextNumber;
        }


        private bool CheckDuplicateNumber(INumberedEntity entity, string nextNumber)
        {
            using (var command = _uow.Connection.CreateCommand())
            {
                command.CommandText = $@"SELECT
                    (CASE
                WHEN EXISTS(
                    SELECT NULL AS [EMPTY]
                        FROM [{_uow.Entry(entity).Metadata.Relational().TableName}] AS [t0]
                        WHERE [t0].[Number] = '{nextNumber}' 
                ) THEN 1
                ELSE 0
                END) [Value]";

                command.Transaction = _uow.Transaction.GetDbTransaction();

                var result = command.ExecuteScalar();

                return Convert.ToBoolean(result);
            }
        }

        private string GenerateNumber(INumberedEntity entity)
        {
            var entityType = entity.GetType();
            var entityName = $"{entityType.FullName}";
            var lockKey = entityName;

            if (_tenant.HasValue)
            {
                lockKey = $"Tenant_{_tenant.Value.Name}_{lockKey}";
            }

            var option = _options.Value.NumberedEntityMap[entityType];

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