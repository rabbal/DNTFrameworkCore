using DNTFrameworkCore.TestWebApp.Domain.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RFrameworkCore.TestWebApp.Data.Mappings.Tasks
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(t => t.Description).HasMaxLength(Task.MaxDescriptionLength);
            builder.Property(t => t.Title).HasMaxLength(Task.MaxTitleLength).IsRequired();
            builder.Property(t => t.NormalizedTitle).HasMaxLength(Task.MaxTitleLength).IsRequired();

            builder.HasIndex(t => t.NormalizedTitle).HasName("UIX_Task_NormalizedTitle").IsUnique();

            builder.ToTable(nameof(Task));
        }
    }
}