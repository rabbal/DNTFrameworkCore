using System;
using System.Data;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Transaction
{
    internal class TransactionProvider<TDbContext> : ITransactionProvider
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public TransactionProvider(TDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ITransaction CurrentTransaction => _context.Database.CurrentTransaction == null
            ? null
            : new DbContextTransactionWrapper(_context.Database.CurrentTransaction);

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (CurrentTransaction == null)
            {
                _context.Database.BeginTransaction(isolationLevel);
            }

            return CurrentTransaction;
        }
    }
}