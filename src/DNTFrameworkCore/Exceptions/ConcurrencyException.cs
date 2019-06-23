using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class ConcurrencyException : FrameworkException
    {
        public ConcurrencyException() : base()
        {
        }
        public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}