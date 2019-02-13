using System.Data;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Transaction
{
    public interface ITransactionProvider : IScopedDependency
    {
        ITransaction CurrentTransaction { get; }
        ITransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}