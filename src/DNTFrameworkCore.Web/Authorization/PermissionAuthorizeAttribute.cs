using System;
using DNTFrameworkCore.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DNTFrameworkCore.Web.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public PermissionAuthorizeAttribute(params string[] permissions)
        {
            Policy = $"{PermissionConstant.PolicyPrefix}{string.Join(PermissionConstant.PolicyNameSplitSymbol, permissions)}";
        }
    }
}