using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DomainException : Exception
    {
        public string Details { get; }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, string details) : base(message)
        {
            Details = details;
        }

        public DomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        
        public DomainException(string message, string details, Exception innerException)
            : base(message, innerException)
        {
            Details = details;
        }
    }
}