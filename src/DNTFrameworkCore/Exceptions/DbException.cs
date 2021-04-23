using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DbException : Exception
    {
        public DbException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}