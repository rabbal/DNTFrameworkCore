using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Configuration
{
    internal sealed class KeyValueService : ApplicationService, IKeyValueService
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<KeyValue> _values;

        public KeyValueService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _values = _uow.Set<KeyValue>();
        }

        public async Task SaveValueAsync(string key, string value)
        {
            var record = await _values.FirstOrDefaultAsync(v => v.Key == key);
            if (record == null)
            {
                _values.Add(new KeyValue
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
            var record = await _values.FirstOrDefaultAsync(v => v.Key == key);
            return record == null ? Maybe<string>.None : record.Value;
        }
    }
}