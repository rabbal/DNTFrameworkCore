using System.Threading.Tasks;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="ISettingService"/>.
    /// </summary>
    public static class SettingServiceExtensions
    {
        /// <summary>
        /// Gets value of a setting in given type (<see cref="T"/>).
        /// </summary>
        /// <typeparam name="T">Type of the setting to get</typeparam>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Value of the setting</returns>
        public static async Task<T> FindSettingValueAsync<T>(this ISettingService settingManager, string name)
            where T : struct
        {
            return (await settingManager.ReadValueAsync(name)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for the application level.
        /// </summary>
        /// <param name="settingManager">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <returns>Current value of the setting for the application</returns>
        public static async Task<T> ReadValueForApplicationAsync<T>(this ISettingService settingManager, string name)
            where T : struct
        {
            return (await settingManager.ReadValueForApplicationAsync(name)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a tenant level.
        /// It gets the setting value, overwritten by given tenant.
        /// </summary>
        /// <param name="service">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <returns>Current value of the setting</returns>
        public static async Task<T> ReadValueForTenantAsync<T>(this ISettingService service, string name, long tenantId)
            where T : struct
        {
            return (await service.ReadValueForTenantAsync(name, tenantId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="service">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> ReadValueForUserAsync<T>(this ISettingService service, string name, int? tenantId,
            long userId)
            where T : struct
        {
            return (await service.ReadValueForUserAsync(name, tenantId, userId)).To<T>();
        }

        /// <summary>
        /// Gets current value of a setting for a user level.
        /// It gets the setting value, overwritten by given tenant and user.
        /// </summary>
        /// <param name="service">Setting manager</param>
        /// <param name="name">Unique name of the setting</param>
        /// <param name="user">User</param>
        /// <returns>Current value of the setting for the user</returns>
        public static async Task<T> ReadValueForUserAsync<T>(this ISettingService service, string name,
            UserIdentifier user)
            where T : struct
        {
            return (await service.ReadValueForUserAsync(name, user)).To<T>();
        }
    }
}