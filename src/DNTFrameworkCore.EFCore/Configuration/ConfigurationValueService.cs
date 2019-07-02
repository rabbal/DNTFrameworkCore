using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Configuration
{
    internal class ConfigurationValueService : ApplicationService, IConfigurationValueService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<ConfigurationValue> _values;

        public ConfigurationValueService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _values = _uow.Set<ConfigurationValue>();
        }

        public async Task SaveValueAsync(string key, string value)
        {
            var record = await _values.SingleOrDefaultAsync(v => v.Key == key);
            if (record == null)
            {
                _values.Add(new ConfigurationValue
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                record.Value = value;
            }

            await _uow.SaveChangesAsync();
        }

        public async Task<Maybe<string>> FindValueAsync(string key)
        {
            var record = await _values.SingleOrDefaultAsync(v => v.Key == key);
            return record == null ? Maybe<string>.None : record.Value;
        }
    }
}