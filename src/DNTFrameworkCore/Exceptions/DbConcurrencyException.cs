using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DbConcurrencyException : DbException
    {
        public DbConcurrencyException() : base(string.Empty, null)
        {
        }

        public DbConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}