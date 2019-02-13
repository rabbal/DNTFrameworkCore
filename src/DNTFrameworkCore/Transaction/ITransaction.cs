using System;
using System.Data.Common;

namespace DNTFrameworkCore.Transaction
{
    public interface ITransaction
    {
        Guid TransactionId { get; }
        DbTransaction DbTransaction { get; }
        void Commit();
        void Rollback();
    }
}