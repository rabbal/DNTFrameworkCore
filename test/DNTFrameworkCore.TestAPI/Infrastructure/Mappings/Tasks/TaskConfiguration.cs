using DNTFrameworkCore.TestAPI.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DNTFrameworkCore.TestAPI.Infrastructure.Mappings.Tasks
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(t => t.Description).HasMaxLength(Task.MaxDescriptionLength);
            builder.Property(t => t.Title).HasMaxLength(Task.MaxTitleLength).IsRequired();
            builder.Property(t => t.NormalizedTitle).HasMaxLength(Task.MaxTitleLength).IsRequired();
            builder.Property(t => t.Number).IsRequired().HasMaxLength(50).Metadata
                .AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.HasIndex(t => new {t.Number, t.BranchId}).HasName("UIX_Task_Number_BranchId");
            builder.HasIndex(t => t.NormalizedTitle).HasName("UIX_Task_NormalizedTitle").IsUnique();

            builder.ToTable(nameof(Task));
        }
    }
}