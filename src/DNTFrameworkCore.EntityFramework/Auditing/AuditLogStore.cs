using System;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EntityFramework.Auditing
{
    public class AuditLogStore<TContext> : IAuditingStore, ITransientDependency
        where TContext : DbContext
    {
        private readonly IServiceProvider _provider;

        public AuditLogStore(IServiceProvider provider)
        {
            Guard.ArgumentNotNull(provider, nameof(provider));

            _provider = provider;
        }

        public void Save(AuditInfo auditInfo)
        {
            using (var serviceScope = _provider.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TContext>();

                context.Set<AuditLog>().Add(AuditLog.CreateFromAuditInfo(auditInfo));
                context.SaveChanges();
            }
        }
    }
}