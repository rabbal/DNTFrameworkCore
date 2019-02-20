using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static void ApplyChanges(this IUnitOfWork uow, IEnumerable<ITrackedEntity> roots)
        {
            foreach (var root in roots)
                uow.ApplyChanges(root);
        }

        public static void AcceptChanges(this IUnitOfWork uow, IEnumerable<ITrackedEntity> roots)
        {
            foreach (var root in roots)
                uow.AcceptChanges(root);
        }

        public static TResult RunInTransaction<TResult>(this IUnitOfWork uow, Func<TResult> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using (var transaction = uow.BeginTransaction(isolationLevel))
            {
                var result = action.Invoke();
                transaction.Commit();
                return result;
            }
        }

        public static async Task<TResult> RunInTransactionAsync<TResult>(this IUnitOfWork uow,
            Func<Task<TResult>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            using (var transaction = await uow.BeginTransactionAsync(isolationLevel))
            {
                var result = await action.Invoke();
                transaction.Commit();
                return result;
            }
        }
    }
}