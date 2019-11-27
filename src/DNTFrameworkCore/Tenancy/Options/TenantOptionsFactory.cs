using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Tenancy.Options
{
    /// <summary>
    /// Create a new options instance with configuration applied
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    internal sealed class TenantOptionsFactory<TOptions> : IOptionsFactory<TOptions>
        where TOptions : class, new()
    {
        private readonly IEnumerable<IConfigureOptions<TOptions>> _setups;
        private readonly IEnumerable<IPostConfigureOptions<TOptions>> _postSetups;
        private readonly ITenantSession _tenantSession;
        private readonly Action<TOptions, Tenant> _tenantSetup;

        public TenantOptionsFactory(
            IEnumerable<IConfigureOptions<TOptions>> setups,
            IEnumerable<IPostConfigureOptions<TOptions>> postSetups, 
            Action<TOptions, Tenant> tenantSetup,
            ITenantSession tenantSession)
        {
            _setups = setups;
            _postSetups = postSetups;
            _tenantSession = tenantSession;
            _tenantSetup = tenantSetup;
        }

        /// <summary>
        /// Create a new options instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TOptions Create(string name)
        {
            var options = new TOptions();

            foreach (var setup in _setups)
            {
                if (setup is IConfigureNamedOptions<TOptions> namedSetup)
                {
                    namedSetup.Configure(name, options);
                }
                else
                {
                    setup.Configure(options);
                }
            }

//            if (_tenantSession.Tenant != null)
//            {
//                _tenantSetup(options, _tenantSession.Tenant);
//            }

            foreach (var postConfig in _postSetups)
            {
                postConfig.PostConfigure(name, options);
            }

            return options;
        }
    }
}