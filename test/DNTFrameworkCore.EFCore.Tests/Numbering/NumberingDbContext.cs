using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.EntityFramework.SqlServer.Numbering;
using DNTFrameworkCore.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EntityFramework.Tests.Numbering
{
    public class NumberingDbContext : DbContextCore
    {
        public NumberingDbContext(IHookEngine hookEngine,
            IUserSession session,
            DbContextOptions<NumberingDbContext> options) : base(hookEngine, session, options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TestTaskConfiguration());
            modelBuilder.ApplyNumberedEntityConfiguration();

            base.OnModelCreating(modelBuilder);
        }
    }

    public class TestTaskConfiguration : IEntityTypeConfiguration<TestTask>
    {
        public void Configure(EntityTypeBuilder<TestTask> builder)
        {
            builder.Property(t => t.Number).HasMaxLength(50).IsRequired();

            builder.ToTable(nameof(TestTask));
        }
    }
}