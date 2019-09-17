using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class ConcurrencyException : DbUpdateException
    {
        public ConcurrencyException() : base(string.Empty, null)
        {
        }

        public ConcurrencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}