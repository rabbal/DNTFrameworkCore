//using System;
//using System.Text;
//using System.Threading.Tasks;
//using DNTFramework.Application.Navigation;
//using DNTFramework.Dependency;
//using DNTFramework.Runtime;
//
//namespace DNTFramework.Web.Navigation
//{
//    internal class NavigationScriptManager : INavigationScriptManager, ITransientDependency
//    {
//        private readonly IUserNavigationManager _userNavigationManager;
//        private readonly ICurrentSession _session;
//
//        public NavigationScriptManager(IUserNavigationManager userNavigationManager, ICurrentSession session)
//        {
//            _userNavigationManager =
//                userNavigationManager ?? throw new ArgumentNullException(nameof(userNavigationManager));
//            _session = session ?? throw new ArgumentNullException(nameof(session));
//        }
//
//        public async Task<string> GetScriptAsync()
//        {
//            var userMenus = await _userNavigationManager.GetMenusAsync(_session.ToUserIdentifier());
//
//            var sb = new StringBuilder();
//            sb.AppendLine("(function() {");
//
//            sb.AppendLine("    abp.nav = {};");
//            sb.AppendLine("    abp.nav.menus = {");
//
//            for (var i = 0; i < userMenus.Count; i++)
//            {
//                AppendMenu(sb, userMenus[i]);
//                if (userMenus.Count - 1 > i)
//                {
//                    sb.Append(" , ");
//                }
//            }
//
//            sb.AppendLine("    };");
//
//            sb.AppendLine("})();");
//
//            return sb.ToString();
//        }
//
//        private static void AppendMenu(StringBuilder sb, UserMenu menu)
//        {
//            sb.AppendLine("        '" + menu.Name + "': {");
//
//            sb.AppendLine("            name: '" + menu.Name + "',");
//
//            if (menu.DisplayName != null)
//            {
//                sb.AppendLine("            displayName: '" + menu.DisplayName + "',");
//            }
//
//            if (menu.CustomData != null)
//            {
//                sb.AppendLine("            customData: " + menu.CustomData.ToJsonString(true) + ",");
//            }
//
//            sb.Append("            items: ");
//
//            if (menu.Items.Count <= 0)
//            {
//                sb.AppendLine("[]");
//            }
//            else
//            {
//                sb.Append("[");
//                for (var i = 0; i < menu.Items.Count; i++)
//                {
//                    AppendMenuItem(16, sb, menu.Items[i]);
//                    if (menu.Items.Count - 1 > i)
//                    {
//                        sb.Append(" , ");
//                    }
//                }
//
//                sb.AppendLine("]");
//            }
//
//            sb.AppendLine("            }");
//        }
//
//        private static void AppendMenuItem(int indentLength, StringBuilder sb, UserMenuItem menuItem)
//        {
//            sb.AppendLine("{");
//
//            sb.AppendLine(new string(' ', indentLength + 4) + "name: '" + menuItem.Name + "',");
//            sb.AppendLine(new string(' ', indentLength + 4) + "order: " + menuItem.Order + ",");
//
//            if (!string.IsNullOrEmpty(menuItem.Icon))
//            {
//                sb.AppendLine(new string(' ', indentLength + 4) + "icon: '" + menuItem.Icon.Replace("'", @"\'") + "',");
//            }
//
//            if (!string.IsNullOrEmpty(menuItem.Url))
//            {
//                sb.AppendLine(new string(' ', indentLength + 4) + "url: '" + menuItem.Url.Replace("'", @"\'") + "',");
//            }
//
//            if (menuItem.DisplayName != null)
//            {
//                sb.AppendLine(new string(' ', indentLength + 4) + "displayName: '" +
//                              menuItem.DisplayName.Replace("'", @"\'") + "',");
//            }
//
//            if (menuItem.CustomData != null)
//            {
//                sb.AppendLine(new string(' ', indentLength + 4) + "customData: " +
//                              menuItem.CustomData.ToJsonString(true) + ",");
//            }
//
//            if (menuItem.Target != null)
//            {
//                sb.AppendLine(new string(' ', indentLength + 4) + "target: '" + menuItem.Target.Replace("'", @"\'") +
//                              "',");
//            }
//
//            sb.AppendLine(new string(' ', indentLength + 4) + "isEnabled: " +
//                          menuItem.Enabled.ToString().ToLowerInvariant() + ",");
//            sb.AppendLine(new string(' ', indentLength + 4) + "isVisible: " +
//                          menuItem.Visible.ToString().ToLowerInvariant() + ",");
//
//            sb.Append(new string(' ', indentLength + 4) + "items: [");
//
//            for (var i = 0; i < menuItem.Items.Count; i++)
//            {
//                AppendMenuItem(24, sb, menuItem.Items[i]);
//                if (menuItem.Items.Count - 1 > i)
//                {
//                    sb.Append(" , ");
//                }
//            }
//
//            sb.AppendLine("]");
//
//            sb.Append(new string(' ', indentLength) + "}");
//        }
//    }
//
//    /// <summary>
//    /// Used to generate navigation scripts.
//    /// </summary>
//    public interface INavigationScriptManager
//    {
//        /// <summary>
//        /// Used to generate navigation scripts.
//        /// </summary>
//        /// <returns></returns>
//        Task<string> GetScriptAsync();
//    }
//}