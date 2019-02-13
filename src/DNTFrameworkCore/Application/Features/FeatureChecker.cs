using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Application.Features
{
    public interface IFeatureChecker : IScopedDependency
    {
        Task<string> ReadValueAsync(string name);
        Task<string> ReadValueAsync(long tenantId, string name);
    }

    internal class FeatureChecker : IFeatureChecker
    {
        private readonly IFeatureValueStore _store;
        private readonly IFeatureService _manager;
        private readonly IUserSession _session;

        public FeatureChecker(IFeatureValueStore store, IFeatureService manager, IUserSession session)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<string> ReadValueAsync(long tenantId, string name)
        {
            var feature = _manager.Find(name);

            if (!feature.HasValue) throw new FrameworkException($"there is no feature with name: {name}");

            var value = await _store.ReadValueAsync(tenantId, feature.Value);

            return value ?? feature.Value.DefaultValue;
        }

        /// <inheritdoc/>
        public Task<string> ReadValueAsync(string name)
        {
            if (!_session.TenantId.HasValue)
            {
                throw new FrameworkException(
                    "FeatureChecker can not get a feature value by name. TenantId is not set in the IUserSession!");
            }

            return ReadValueAsync(_session.TenantId.Value, name);
        }
    }
}