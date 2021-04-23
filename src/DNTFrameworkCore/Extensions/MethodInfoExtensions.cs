using System.Reflection;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Reflection;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Extensions
{
    public static class MethodInfoExtensions
    {
        public static bool ValidationIgnored(this MethodInfo method)
        {
            var parameters = method.GetParameters();
            return !method.IsPublic || parameters.IsNullOrEmpty() || IsValidationSkipped(method);
        }

        private static bool IsValidationSkipped(MemberInfo method)
        {
            if (method.IsDefined(typeof(EnableValidationAttribute), true))
            {
                return false;
            }

            return ReflectionHelper
                       .GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<SkipValidationAttribute>(method) != null;
        }
    }
}