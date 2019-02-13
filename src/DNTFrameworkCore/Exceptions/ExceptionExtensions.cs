using System;
using System.Text;

namespace DNTFrameworkCore.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string ReadExceptionDetails(this Exception ex)
        {
            var errorString = new StringBuilder();
            errorString.AppendLine("An error occurred. ");
            var inner = ex;
            while (inner != null)
            {
                errorString.Append("Error Message:");
                errorString.AppendLine(inner.Message);
                errorString.Append("Stack Trace:");
                errorString.AppendLine(inner.StackTrace);
                inner = inner.InnerException;
            }

            return errorString.ToString();
        }
    }
}