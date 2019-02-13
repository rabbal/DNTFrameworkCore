using System.Reflection;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Threading
{
    public static class AsyncHelper
    {
        /// <summary>
        /// Checks if given method is an async method.
        /// </summary>
        /// <param name="method">A method to check</param>
        public static bool IsAsync(this MethodInfo method)
        {
            return method.ReturnType == typeof(Task) ||
                   (method.ReturnType.GetTypeInfo().IsGenericType &&
                    method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }
    }
}