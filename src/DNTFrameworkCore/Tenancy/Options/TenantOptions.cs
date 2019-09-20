using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Tenancy.Options
{
    /// <summary>
    /// Make IOptions tenant aware
    /// </summary>
    internal sealed class TenantOptions<TOptions> : IOptionsSnapshot<TOptions> where TOptions : class, new()
    {
        private readonly IOptionsFactory<TOptions> _factory;
        private readonly IOptionsMonitorCache<TOptions> _cache;

        public TenantOptions(IOptionsFactory<TOptions> factory, IOptionsMonitorCache<TOptions> cache)
        {
            _factory = factory;
            _cache = cache;
        }

        public TOptions Value => Get(Microsoft.Extensions.Options.Options.DefaultName);

        public TOptions Get(string name)
        {
            return _cache.GetOrAdd(name, () => _factory.Create(name));
        }
    }
}