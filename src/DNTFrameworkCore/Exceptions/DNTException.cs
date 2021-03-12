using System;
using System.Runtime.Serialization;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class DNTException : Exception
    {
        public DNTException()
        {
        }

        public DNTException(string message)
            : base(message)
        {
        }

        public DNTException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DNTException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }
    }
}