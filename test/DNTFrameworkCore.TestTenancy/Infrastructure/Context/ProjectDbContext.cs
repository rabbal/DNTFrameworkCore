using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;

namespace DNTFrameworkCore.TestTenancy.Infrastructure.Context
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
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.AddJsonFields();
            modelBuilder.AddTrackingFields<long>();
            modelBuilder.AddTenancyField<long>();
            modelBuilder.AddIsDeletedField();
            modelBuilder.AddRowVersionField();
            modelBuilder.AddRowIntegrityField();
            modelBuilder.AddRowLevelSecurityField<long>();

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnSaveCompleted(EntityChangeContext context)
        {
            this.GetService<IEFCacheServiceProvider>()
                .InvalidateCacheDependencies(context.EntityNames.ToArray());
        }
    }
}