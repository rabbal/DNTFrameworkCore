using System;
using System.Data.Common;

namespace DNTFrameworkCore.Transaction
{
    internal class NullTransaction : ITransaction
    {
        public Guid TransactionId => Guid.Empty;
        public DbTransaction DbTransaction => null;

        public void Commit()
        {
        }

        public void Rollback()
        {
        }
    }
}