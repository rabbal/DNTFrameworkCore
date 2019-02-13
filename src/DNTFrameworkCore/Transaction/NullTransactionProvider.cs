using System.Data;

namespace DNTFrameworkCore.Transaction
{
    internal class NullTransactionProvider : ITransactionProvider
    {
        public static readonly NullTransactionProvider Instance = new NullTransactionProvider();
        public ITransaction CurrentTransaction { get; private set; }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return CurrentTransaction = new NullTransaction();
        }
    }
}