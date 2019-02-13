using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.TestWebApp.Infrastructure.Mappings.Blogging;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Infrastructure.Context
{
    public class ProjectDbContext : DbContextCore
    {
        public ProjectDbContext(DbContextCoreDependency<ProjectDbContext> dependency) : base(dependency)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BlogConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}