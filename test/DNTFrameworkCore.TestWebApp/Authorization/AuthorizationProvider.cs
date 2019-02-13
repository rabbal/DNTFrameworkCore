using System.Collections.Generic;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Localization;

namespace DNTFrameworkCore.TestWebApp.Authorization
{
    public class AuthorizationProvider : IAuthorizationProvider
    {
        public IEnumerable<Permission> ProvidePermissions()
        {
            yield return Permission.CreatePermission(PermissionNames.Administration, L(PermissionNames.Administration))
                .CreateChildPermission(PermissionNames.Administration_Blogs_View,
                    L(PermissionNames.Administration_Blogs_View))
                .CreateChildPermission(PermissionNames.Administration_Blogs_Create,
                    L(PermissionNames.Administration_Blogs_Create))
                .CreateChildPermission(PermissionNames.Administration_Blogs_Edit,
                    L(PermissionNames.Administration_Blogs_Edit))
                .CreateChildPermission(PermissionNames.Administration_Blogs_Delete,
                    L(PermissionNames.Administration_Blogs_Delete));
        }

        private ILocalizableString L(string name)
        {
            return new LocalizableString(name, "PermissionsResource") {ResourceLocation = "Resources"};
        }
    }
}