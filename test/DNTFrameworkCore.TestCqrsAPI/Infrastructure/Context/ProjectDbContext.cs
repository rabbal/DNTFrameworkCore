using System.Collections.Generic;
using DNTFrameworkCore.EFCore.Caching;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Cryptography;
using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure.Context
{
    public class ProjectDbContext : DbContextCore
    {
        public ProjectDbContext(
            DbContextOptions<ProjectDbContext> options,
            IEnumerable<IHook> hooks) : base(options, hooks)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyLogConfiguration();
            modelBuilder.ApplyNumberedEntityConfiguration();
            modelBuilder.ApplyProtectionKeyConfiguration();
            modelBuilder.ApplySqlCacheConfiguration();
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.NormalizeDateTime();
            modelBuilder.NormalizeDecimal();

            base.OnModelCreating(modelBuilder);
        }
    }
}