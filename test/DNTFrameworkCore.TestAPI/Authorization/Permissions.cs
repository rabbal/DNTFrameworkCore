using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Localization;

namespace DNTFrameworkCore.TestAPI.Authorization
{
    public static class Permissions
    {
        public static IReadOnlyList<Permission> List()
        {
            return new List<Permission>
            {
                Permission.CreatePermission(PermissionNames.Blogs_View,
                    L(PermissionNames.Blogs_View)),
                Permission.CreatePermission(PermissionNames.Blogs_Create,
                    L(PermissionNames.Blogs_Create)),
                Permission.CreatePermission(PermissionNames.Blogs_Edit,
                    L(PermissionNames.Blogs_Edit)),
                Permission.CreatePermission(PermissionNames.Blogs_Delete,
                    L(PermissionNames.Blogs_Delete)),

                Permission.CreatePermission(PermissionNames.Roles_View,
                    L(PermissionNames.Roles_View)),
                Permission.CreatePermission(PermissionNames.Roles_Create,
                    L(PermissionNames.Roles_Create)),
                Permission.CreatePermission(PermissionNames.Roles_Edit,
                    L(PermissionNames.Roles_Edit)),
                Permission.CreatePermission(PermissionNames.Roles_Delete,
                    L(PermissionNames.Roles_Delete)),

                Permission.CreatePermission(PermissionNames.Users_View,
                    L(PermissionNames.Users_View)),
                Permission.CreatePermission(PermissionNames.Users_Create,
                    L(PermissionNames.Users_Create)),
                Permission.CreatePermission(PermissionNames.Users_Edit,
                    L(PermissionNames.Users_Edit)),
                Permission.CreatePermission(PermissionNames.Users_Delete,
                    L(PermissionNames.Users_Delete)),

                Permission.CreatePermission(PermissionNames.Tasks_View,
                    L(PermissionNames.Tasks_View)),
                Permission.CreatePermission(PermissionNames.Tasks_Create,
                    L(PermissionNames.Tasks_Create)),
                Permission.CreatePermission(PermissionNames.Tasks_Edit,
                    L(PermissionNames.Tasks_Edit)),
                Permission.CreatePermission(PermissionNames.Tasks_Delete,
                    L(PermissionNames.Tasks_Delete)),
            };
        }

        public static IEnumerable<string> Names()
        {
            return List().Select(permission => permission.Name).ToList();
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, "PermissionsResource", "Resources");
        }
    }
}