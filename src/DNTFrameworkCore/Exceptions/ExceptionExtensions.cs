using System;
using System.Text;

namespace DNTFrameworkCore.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string ToStringFormat(this Exception ex)
        {
            var builder = new StringBuilder();
            builder.AppendLine("An error occurred. ");
            
            var inner = ex;
            while (inner != null)
            {
                builder.Append("Error Message:");
                builder.AppendLine(inner.Message);
                builder.Append("Stack Trace:");
                builder.AppendLine(inner.StackTrace);
                inner = inner.InnerException;
            }

            return builder.ToString();
        }
    }
}