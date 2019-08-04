using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Authorization
{
    public static class PermissionExtensions
    {
        public static string PackPermissionsToString(this IEnumerable<string> permissions)
        {
            return string.Join(PermissionConstant.PackingSymbol, permissions);
        }

        public static IEnumerable<string> UnpackPermissionsFromString(this string packedPermissions)
        {
            if (packedPermissions == null) throw new ArgumentNullException(nameof(packedPermissions));

            return packedPermissions.Split(new[] {PermissionConstant.PackingSymbol}, StringSplitOptions.None);
        }

        public static IEnumerable<string> ExtractPermissionsFromPolicyName(this string policyName)
        {
            return policyName.Substring(PermissionConstant.PolicyPrefix.Length)
                .Split(new[] {PermissionConstant.PolicyNameSplitSymbol}, StringSplitOptions.None);
        }
    }
}