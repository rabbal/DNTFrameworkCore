using System.Linq;
using DNTFrameworkCore.EntityFramework.Auditing;
using DNTFrameworkCore.EntityFramework.Caching;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.DataProtection;
using DNTFrameworkCore.EntityFramework.Logging;
using DNTFrameworkCore.EntityFramework.SqlServer.Numbering;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Blogging;
using DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Catalog;
using DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Identity;
using DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Invoices;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using RFrameworkCore.TestWebApp.Data.Mappings.Tasks;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Context
{
    public class ProjectDbContext : DbContextCore
    {
        public ProjectDbContext(
            IHookEngine hookEngine,
            IUserSession session,
            DbContextOptions<ProjectDbContext> options) : base(hookEngine, session, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlogConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserPermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceItemConfiguration());
            modelBuilder.ApplyLogConfiguration();
            modelBuilder.ApplyNumberedEntityConfiguration();
            modelBuilder.ApplyDataProtectionKeyConfiguration();
            modelBuilder.ApplySqlCacheConfiguration();
            modelBuilder.ApplyAuditLogConfiguration();

            base.OnModelCreating(modelBuilder);
        }

        protected override void AfterSaveChanges(SaveChangeContext context)
        {
            this.GetService<IEFCacheServiceProvider>()
                .InvalidateCacheDependencies(context.ChangedEntityNames.ToArray());
        }
    }
}