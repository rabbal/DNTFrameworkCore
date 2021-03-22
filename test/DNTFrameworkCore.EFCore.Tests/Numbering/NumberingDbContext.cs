using System.Collections.Generic;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.EFCore.Tests.Numbering
{
    public class NumberingDbContext : DbContextCore
    {
        public NumberingDbContext(DbContextOptions<NumberingDbContext> options, IEnumerable<IHook> hooks) : base(
            options, hooks)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TestTaskConfiguration());
            modelBuilder.ApplyNumberedEntityConfiguration();
            modelBuilder.AddTenancyField<long>();
            modelBuilder.AddTrackingFields<long>();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TestTaskConfiguration : IEntityTypeConfiguration<NumberingTestEntity>
    {
        public void Configure(EntityTypeBuilder<NumberingTestEntity> builder)
        {
            builder.Property(t => t.Number).HasMaxLength(50).IsRequired();

            builder.ToTable(nameof(NumberingTestEntity));
        }
    }
}