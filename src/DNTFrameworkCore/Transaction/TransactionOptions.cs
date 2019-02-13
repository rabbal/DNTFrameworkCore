using System;
using System.Data;

namespace DNTFrameworkCore.Transaction
{
    public class TransactionOptions
    {
        public TimeSpan? Timeout { get; set; }
        
        public IsolationLevel? IsolationLevel { get; set; }
    }
}