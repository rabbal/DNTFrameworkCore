using System;
using System.Linq;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.Context.Extensions;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EntityFramework.SqlServer.Numbering
{
    internal class NumberingPreInsertHook : PreInsertHook<INumberedEntity>
    {
        private readonly IUnitOfWork _uow;
        private readonly IOptions<NumberingOptions> _options;

        public NumberingPreInsertHook(IUnitOfWork uow, IOptions<NumberingOptions> options)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        protected override void Hook(INumberedEntity entity, HookEntityMetadata metadata)
        {
            if (!entity.Number.IsEmpty()) return;

            bool retry;
            string nextNumber;

            do
            {
                nextNumber = GenerateNumber(entity);
                var exists = CheckDuplicateNumber(entity, nextNumber);
                retry = exists;
            } while (retry);

            entity.Number = nextNumber;
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

                command.Transaction = _uow.Transaction;

                var result = command.ExecuteScalar();

                return Convert.ToBoolean(result);
            }
        }

        private string GenerateNumber(INumberedEntity entity)
        {
            var entityType = entity.GetType();
            var entityName = $"{entityType.FullName}";
            var lockKey = $"Tenant_{_uow.TenantId}_" + entityName;
            var option = _options.Value.NumberedEntityMap[entityType];

            _uow.AcquireDistributedLock(lockKey);

            var nextNumber = option.Start.ToString();

            var numberedEntity = _uow.Set<NumberedEntity>().AsNoTracking()
                .FirstOrDefault(a => a.EntityName == entityName);
            if (numberedEntity == null)
            {
                _uow.ExecuteSqlCommand(
                    "INSERT INTO [dbo].[NumberedEntity]([EntityName], [NextNumber], [TenantId]) VALUES(@p0,@p1,@p2)",
                    entityName,
                    option.Start + option.IncrementBy, _uow.TenantId);
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