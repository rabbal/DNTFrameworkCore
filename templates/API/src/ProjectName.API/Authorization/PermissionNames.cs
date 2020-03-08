using System.Collections.Generic;

namespace ProjectName.API.Authorization
{
    public static class PermissionNames
    {
        public static readonly ISet<string> NameList = new HashSet<string>
        {
            Users_View,
            Users_Create,
            Users_Edit,
            Users_Delete,
            Roles_View,
            Roles_Create,
            Roles_Edit,
            Roles_Delete
        };

        public const string Users_View = nameof(Users_View);
        public const string Users_Create = nameof(Users_Create);
        public const string Users_Edit = nameof(Users_Edit);
        public const string Users_Delete = nameof(Users_Delete);

        public const string Roles_View = nameof(Roles_View);
        public const string Roles_Create = nameof(Roles_Create);
        public const string Roles_Edit = nameof(Roles_Edit);
        public const string Roles_Delete = nameof(Roles_Delete);
    }
}