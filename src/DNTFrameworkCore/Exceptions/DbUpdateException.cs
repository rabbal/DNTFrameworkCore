using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DbUpdateException : FrameworkException
    {
        public DbUpdateException(string message, Exception innerException) 
        : base(message, innerException)
        {
        }
    }
}