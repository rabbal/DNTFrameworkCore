using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.EFCore.Caching;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Converters.Json;
using DNTFrameworkCore.EFCore.Context.Extensions;
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
        public ProjectDbContext(DbContextOptions<ProjectDbContext> options, IEnumerable<IHook> hooks) : base(options,
            hooks)
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
            modelBuilder.AddIsDeletedField();
            modelBuilder.AddRowVersionField();
            modelBuilder.AddRowIntegrityField();
            modelBuilder.AddRowLevelSecurityField<long>();
            
            modelBuilder.SpecifyDateTimeKind();
            modelBuilder.SpecifyDecimalPrecision();
            
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnSaveCompleted(EntityChangeContext context)
        {
            this.GetService<IEFCacheServiceProvider>()
                .InvalidateCacheDependencies(context.EntityNames.ToArray());
        }
    }
}