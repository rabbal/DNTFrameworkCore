using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    internal class SettingStore<TContext> : ISettingStore, ITransientDependency
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly DbSet<Setting> _settings;

        public SettingStore(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _settings = context.Set<Setting>();
        }

        public async Task<Maybe<SettingInfo>> FindAsync(long? tenantId, long? userId, string name)
        {
            var info = await _settings.FirstOrDefaultAsync(s =>
                s.UserId == userId && s.Name == name && s.TenantId == tenantId);

            return info != null ? info.ToSettingInfo() : Maybe<SettingInfo>.None;
        }

        public async Task DeleteAsync(SettingInfo setting)
        {
            var record = await _settings.FirstOrDefaultAsync(s =>
                s.Name == setting.Name && s.UserId == setting.UserId && s.TenantId == setting.TenantId);

            if (record == null) return;

            _context.Remove(record);
            await _context.SaveChangesAsync();
        }

        public Task CreateAsync(SettingInfo setting)
        {
            _settings.Add(setting.ToSetting());
            return _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(SettingInfo setting)
        {
            var record = await _context.Set<Setting>().FirstOrDefaultAsync(
                s => s.Name == setting.Name && s.UserId == setting.UserId && s.TenantId == setting.TenantId);

            if (record == null) return;

            record.Value = setting.Value;
            await _context.SaveChangesAsync();
        }


        public async Task<List<SettingInfo>> ReadListAsync(long? tenantId, long? userId)
        {
            var settings = await _settings.Where(s => s.TenantId == tenantId && s.UserId == userId).ToListAsync();

            return settings.Select(s => s.ToSettingInfo()).ToList();
        }
    }
}