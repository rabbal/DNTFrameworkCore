using System.Collections.Generic;
using System.Reflection;
using DNTFrameworkCore.EFCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Logging;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.ApplyKeyValueConfiguration();
            //modelBuilder.ApplyProtectionKeyConfiguration();
            //modelBuilder.ApplyNumberedEntityConfiguration();
            //modelBuilder.ApplySqlCacheConfiguration();

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.NormalizeDateTime();
            //modelBuilder.NormalizeDecimalPrecision();

            base.OnModelCreating(modelBuilder);
        }
    }
}