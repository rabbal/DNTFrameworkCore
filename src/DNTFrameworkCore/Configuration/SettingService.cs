using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Caching;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Configuration
{
    internal class SettingService : ISettingService
    {
        private const string ApplicationSettingsCacheKey = "APPLICATION_SETTINGS_CACHE_KEY";
        private const string TenantSettingsCacheKeyTemplate = "TENANT_{0}_SETTINGS_CACHE_KEY";
        private const string UserSettingsCacheKeyTemplate = "USER_{0}_SETTINGS_CACHE_KEY";

        private readonly ISettingDefinitionService _definitionService;
        private readonly IUserSession _session;
        private readonly ISettingStore _store;
        private readonly ICacheService _cache;
        private TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        public SettingService(
            ISettingDefinitionService definitionService,
            IUserSession session,
            ISettingStore store,
            ICacheService cache)
        {
            _definitionService = definitionService ?? throw new ArgumentNullException(nameof(definitionService));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }


        public Task<string> ReadValueAsync(string name)
        {
            return ReadValueInternalAsync(name, _session.TenantId, _session.UserId);
        }

        public Task<string> ReadValueForApplicationAsync(string name)
        {
            return ReadValueInternalAsync(name);
        }

        public Task<string> ReadValueForApplicationAsync(string name, bool fallbackToDefault)
        {
            return ReadValueInternalAsync(name, fallbackToDefault: fallbackToDefault);
        }

        public Task<string> ReadValueForTenantAsync(string name, long tenantId)
        {
            return ReadValueInternalAsync(name, tenantId);
        }

        public Task<string> ReadValueForTenantAsync(string name, long tenantId, bool fallbackToDefault)
        {
            return ReadValueInternalAsync(name, tenantId, fallbackToDefault: fallbackToDefault);
        }

        public Task<string> ReadValueForUserAsync(string name, long? tenantId, long userId)
        {
            return ReadValueInternalAsync(name, tenantId, userId);
        }

        public Task<string> ReadValueForUserAsync(string name, long? tenantId, long userId,
            bool fallbackToDefault)
        {
            return ReadValueInternalAsync(name, tenantId, userId, fallbackToDefault);
        }

        public async Task<IReadOnlyList<ISettingValue>> ReadValuesAsync()
        {
            return await ReadValuesAsync(SettingScopes.Application | SettingScopes.Tenant |
                                         SettingScopes.User);
        }

        public async Task<IReadOnlyList<ISettingValue>> ReadValuesAsync(SettingScopes scopes)
        {
            var settingDefinitions = new Dictionary<string, SettingDefinition>();
            var settingValues = new Dictionary<string, ISettingValue>();

            //Fill all setting with default values.
            foreach (var setting in _definitionService.ReadList())
            {
                settingDefinitions[setting.Name] = setting;
                settingValues[setting.Name] = new SettingValueObject(setting.Name, setting.DefaultValue);
            }

            //Overwrite application settings
            if (scopes.HasFlag(SettingScopes.Application))
            {
                foreach (var settingValue in await ReadValuesForApplicationAsync())
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Application))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        ((setting.Scopes.HasFlag(SettingScopes.Tenant) && _session.TenantId.HasValue) ||
                         (setting.Scopes.HasFlag(SettingScopes.User) && _session.UserId.HasValue)))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            //Overwrite tenant settings
            if (scopes.HasFlag(SettingScopes.Tenant) && _session.TenantId.HasValue)
            {
                foreach (var settingValue in await ReadValuesForTenantAsync(_session.TenantId.Value))
                {
                    var setting = settingDefinitions.GetOrDefault(settingValue.Name);

                    //TODO: Conditions get complicated, try to simplify it
                    if (setting == null || !setting.Scopes.HasFlag(SettingScopes.Tenant))
                    {
                        continue;
                    }

                    if (!setting.IsInherited &&
                        (setting.Scopes.HasFlag(SettingScopes.User) && _session.UserId.HasValue))
                    {
                        continue;
                    }

                    settingValues[settingValue.Name] = new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            if (!scopes.HasFlag(SettingScopes.User) || !_session.UserId.HasValue)
                return settingValues.Values.ToImmutableList();

            //Overwrite user settings
            foreach (var settingValue in await ReadValuesForUserAsync(_session.ToUserIdentifier()))
            {
                var setting = settingDefinitions.GetOrDefault(settingValue.Name);
                if (setting != null && setting.Scopes.HasFlag(SettingScopes.User))
                {
                    settingValues[settingValue.Name] =
                        new SettingValueObject(settingValue.Name, settingValue.Value);
                }
            }

            return settingValues.Values.ToImmutableList();
        }

        public async Task<IReadOnlyList<ISettingValue>> ReadValuesForApplicationAsync()
        {
            return (await GetApplicationSettingsAsync()).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public async Task<IReadOnlyList<ISettingValue>> ReadValuesForTenantAsync(long tenantId)
        {
            return (await GetReadOnlyTenantSettings(tenantId)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public Task<IReadOnlyList<ISettingValue>> ReadValuesForUserAsync(long userId)
        {
            return ReadValuesForUserAsync(new UserIdentifier(_session.TenantId, userId));
        }

        public async Task<IReadOnlyList<ISettingValue>> ReadValuesForUserAsync(UserIdentifier user)
        {
            return (await GetReadOnlyUserSettings(user)).Values
                .Select(setting => new SettingValueObject(setting.Name, setting.Value))
                .ToImmutableList();
        }

        public virtual async Task ChangeSettingForApplicationAsync(string name, string value)
        {
            await SaveValueAsync(name, value, null, null);
            _cache.Remove(ApplicationSettingsCacheKey);
        }

        public virtual async Task ChangeSettingForTenantAsync(long tenantId, string name, string value)
        {
            await SaveValueAsync(name, value, tenantId, null);
            _cache.Remove(string.Format(TenantSettingsCacheKeyTemplate, tenantId));
        }

        public virtual Task ChangeSettingForUserAsync(long userId, string name, string value)
        {
            return ChangeSettingForUserAsync(new UserIdentifier(_session.TenantId, userId), name, value);
        }

        public async Task ChangeSettingForUserAsync(UserIdentifier user, string name, string value)
        {
            await SaveValueAsync(name, value, user.TenantId, user.UserId);
            _cache.Remove(string.Format(UserSettingsCacheKeyTemplate, user.ToUserIdentifierString()));
        }

        private async Task<string> ReadValueInternalAsync(string name, long? tenantId = null, long? userId = null,
            bool fallbackToDefault = true)
        {
            var settingDefinition = _definitionService.Find(name);

            if (!settingDefinition.HasValue) return string.Empty;

            //Get for user if defined
            if (settingDefinition.Value.Scopes.HasFlag(SettingScopes.User) && userId.HasValue)
            {
                var settingValue =
                    await ReadValueForUserOrNullAsync(new UserIdentifier(tenantId, userId.Value), name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.Value.IsInherited)
                {
                    return settingDefinition.Value.DefaultValue;
                }
            }

            //Get for tenant if defined
            if (settingDefinition.Value.Scopes.HasFlag(SettingScopes.Tenant) && tenantId.HasValue)
            {
                var settingValue = await ReadValueForTenantOrNullAsync(tenantId.Value, name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }

                if (!settingDefinition.Value.IsInherited)
                {
                    return settingDefinition.Value.DefaultValue;
                }
            }

            //Get for application if defined
            if (settingDefinition.Value.Scopes.HasFlag(SettingScopes.Application))
            {
                var settingValue = await ReadValueForApplicationOrNullAsync(name);
                if (settingValue != null)
                {
                    return settingValue.Value;
                }

                if (!fallbackToDefault)
                {
                    return null;
                }
            }

            //Not defined, get default value
            return settingDefinition.Value.DefaultValue;
        }

        private async Task SaveValueAsync(string name, string value,
            long? tenantId, long? userId)
        {
            var settingDefinition = _definitionService.Find(name);
            if (!settingDefinition.HasValue)
                throw new InvalidOperationException($"setting with name: {name} not found!");

            var settingValue = await _store.FindAsync(tenantId, userId, name);

            //Determine defaultValue
            var defaultValue = settingDefinition.Value.DefaultValue;

            if (settingDefinition.Value.IsInherited)
            {
                //For Tenant and User, Application's value overrides Setting Definition's default value.
                if (tenantId.HasValue || userId.HasValue)
                {
                    var applicationValue = await ReadValueForApplicationOrNullAsync(name);
                    if (applicationValue != null)
                    {
                        defaultValue = applicationValue.Value;
                    }
                }

                //For User, Tenants's value overrides Application's default value.
                if (userId.HasValue && tenantId.HasValue)
                {
                    var tenantValue = await ReadValueForTenantOrNullAsync(tenantId.Value, name);
                    if (tenantValue != null)
                    {
                        defaultValue = tenantValue.Value;
                    }
                }
            }

            //No need to store on database if the value is the default value
            if (value == defaultValue)
            {
                if (settingValue.HasValue)
                {
                    await _store.DeleteAsync(settingValue.Value);
                }
            }

            //If it's not default value and not stored on database, then insert it
            if (settingValue.HasValue)
            {
                settingValue = new SettingInfo
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Name = name,
                    Value = value
                };

                await _store.CreateAsync(settingValue.Value);
            }

            //It's same value in database, no need to update
            if (settingValue.HasValue && settingValue.Value.Value == value)
            {
                return;
            }

            //Update the setting on database.
            settingValue.Value.Value = value;
            await _store.UpdateAsync(settingValue.Value);
        }

        private async Task<SettingInfo> ReadValueForApplicationOrNullAsync(string name)
        {
            return (await GetApplicationSettingsAsync()).GetOrDefault(name);
        }

        private async Task<SettingInfo> ReadValueForTenantOrNullAsync(long tenantId, string name)
        {
            return (await GetReadOnlyTenantSettings(tenantId)).GetOrDefault(name);
        }

        private async Task<SettingInfo> ReadValueForUserOrNullAsync(UserIdentifier user, string name)
        {
            return (await GetReadOnlyUserSettings(user)).GetOrDefault(name);
        }

        private async Task<Dictionary<string, SettingInfo>> GetApplicationSettingsAsync()
        {
            _cacheDuration = TimeSpan.FromHours(1);
            return await _cache.GetOrAddAsync(ApplicationSettingsCacheKey, async () =>
            {
                var dictionary = new Dictionary<string, SettingInfo>();

                var settingValues = await _store.ReadListAsync(null, null);
                foreach (var settingValue in settingValues)
                {
                    dictionary[settingValue.Name] = settingValue;
                }

                return dictionary;
            }, DateTimeOffset.UtcNow.Add(_cacheDuration));
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyTenantSettings(long tenantId)
        {
            var cachedDictionary = await GetTenantSettingsFromCache(tenantId);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<ImmutableDictionary<string, SettingInfo>> GetReadOnlyUserSettings(UserIdentifier user)
        {
            var cachedDictionary = await GetUserSettingsFromCache(user);
            lock (cachedDictionary)
            {
                return cachedDictionary.ToImmutableDictionary();
            }
        }

        private async Task<Dictionary<string, SettingInfo>> GetTenantSettingsFromCache(long tenantId)
        {
            return await _cache.GetOrAddAsync(
                string.Format(TenantSettingsCacheKeyTemplate, tenantId),
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = await _store.ReadListAsync(tenantId, null);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                }, DateTimeOffset.UtcNow.Add(_cacheDuration));
        }

        private Task<Dictionary<string, SettingInfo>> GetUserSettingsFromCache(UserIdentifier user)
        {
            return _cache.GetOrAddAsync(
                string.Format(TenantSettingsCacheKeyTemplate, user.ToUserIdentifierString()),
                async () =>
                {
                    var dictionary = new Dictionary<string, SettingInfo>();

                    var settingValues = await _store.ReadListAsync(user.TenantId, user.UserId);
                    foreach (var settingValue in settingValues)
                    {
                        dictionary[settingValue.Name] = settingValue;
                    }

                    return dictionary;
                }, DateTimeOffset.UtcNow.Add(_cacheDuration));
        }

        public Task<string> ReadValueForUserAsync(string name, UserIdentifier user)
        {
            Guard.ArgumentNotEmpty(name, nameof(name));
            Guard.ArgumentNotNull(user, nameof(user));

            return ReadValueForUserAsync(name, user.TenantId, user.UserId);
        }

        private class SettingValueObject : ISettingValue
        {
            public string Name { get; }

            public string Value { get; }

            public SettingValueObject(string name, string value)
            {
                Value = value;
                Name = name;
            }
        }
    }
}