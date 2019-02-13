using System;
using System.Data.Common;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore.Storage;

namespace DNTFrameworkCore.EntityFramework.Transaction
{
    internal class DbContextTransactionWrapper : ITransaction
    {
        private readonly IDbContextTransaction _transaction;

        public DbContextTransactionWrapper(IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public Guid TransactionId => Guid.NewGuid();
        public DbTransaction DbTransaction => _transaction.GetDbTransaction();

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }
    }
}