using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.EFCore.Caching;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Converters.Json;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.EFCore.Protection;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Context
{
    public class ProjectDbContext : DbContextCore
    {
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IEnumerable<IHook> hooks) : base(options, hooks)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyLogConfiguration();
            modelBuilder.ApplyNumberedEntityConfiguration();
            modelBuilder.ApplyProtectionKeyConfiguration();
            modelBuilder.ApplySqlCacheConfiguration();
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.AddJsonFields();
            modelBuilder.AddTrackingFields<long>();
            modelBuilder.AddTenancyField<long>();
            modelBuilder.AddSoftDeletedField();
            modelBuilder.AddRowVersionField();
            modelBuilder.AddRowIntegrityField();
            modelBuilder.AddRowLevelSecurityField<long>();

            base.OnModelCreating(modelBuilder);
        }

        protected override void AfterSaveChanges(EntityChangeContext context)
        {
            this.GetService<IEFCacheServiceProvider>()
                .InvalidateCacheDependencies(context.EntityNames.ToArray());
        }
    }
}