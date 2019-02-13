using Microsoft.AspNetCore.Authorization;

namespace DNTFrameworkCore.Web.Authorization
{
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        internal const string PolicyPrefix = "PERMISSION:";

        /// <summary>
        /// Creates a new instance of <see cref="AuthorizeAttribute"/> class.
        /// </summary>
        /// <param name="permissions">A list of permissions to authorize</param>
        public PermissionAuthorizeAttribute(params string[] permissions)
        {
            Policy = $"{PolicyPrefix}{string.Join(",", permissions)}";
        } 
    }
}