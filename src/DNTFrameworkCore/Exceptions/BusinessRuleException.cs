using System;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class BusinessRuleException : Exception
    {
        public string Details { get; }
        public BusinessRuleException(string message) : base(message)
        {
        }
        public BusinessRuleException(string message, string details) : base(message)
        {
            Details = details;
        }
    }
}