using System.Collections.Generic;
using System.Reflection;
using DNTFrameworkCore.EFCore.Caching;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Converters.Json;
using DNTFrameworkCore.EFCore.Context.Extensions;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Cryptography;
using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
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

            modelBuilder.NormalizeDateTime();
            modelBuilder.NormalizeDecimalPrecision(20, 6);


            base.OnModelCreating(modelBuilder);
        }
    }
}