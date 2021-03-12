using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DbException : DNTException
    {
        public DbException(string message, Exception innerException) 
        : base(message, innerException)
        {
        }
    }
}