using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.EFCore.Caching;
using DNTFrameworkCore.EFCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Cryptography;
using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using EFSecondLevelCache.Core.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ProjectName.Infrastructure.Context
{
    public class ProjectNameDbContext : DbContextCore
    {
        public ProjectNameDbContext(
            DbContextOptions<ProjectNameDbContext> options,
            IEnumerable<IHook> hooks) : base(options, hooks)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyLogConfiguration();
            modelBuilder.ApplyNumberedEntityConfiguration();
            modelBuilder.ApplyKeyValueConfiguration();
            modelBuilder.ApplyProtectionKeyConfiguration();
            modelBuilder.ApplySqlCacheConfiguration();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnSaveCompleted(EntityChangeContext context)
        {
            this.GetService<IEFCacheServiceProvider>()
                .InvalidateCacheDependencies(context.EntityNames.ToArray());
            
            base.OnSaveCompleted(context);
        }
    }
}