using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        protected DomainException()
        {
        }

        protected DomainException(string message)
            : base(message)
        {
        }

        protected DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}