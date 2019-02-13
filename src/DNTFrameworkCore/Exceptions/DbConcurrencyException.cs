using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DbConcurrencyException : FrameworkException
    {
        public DbConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}