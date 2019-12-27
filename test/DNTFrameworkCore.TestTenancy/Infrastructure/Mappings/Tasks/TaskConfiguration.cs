using DNTFrameworkCore.TestTenancy.Domain.Tasks;

namespace DNTFrameworkCore.TestTenancy.Infrastructure.Mappings.Tasks
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.Property(t => t.Description).HasMaxLength(Task.MaxDescriptionLength);
            builder.Property(t => t.Title).HasMaxLength(Task.MaxTitleLength).IsRequired();
            builder.Property(t => t.NormalizedTitle).HasMaxLength(Task.MaxTitleLength).IsRequired();
            builder.Property(t => t.Number).IsRequired().HasMaxLength(50).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.HasIndex(t => new {t.Number, t.BranchId}).HasName("UIX_Task_Number_BranchId");
            builder.HasIndex(t => t.NormalizedTitle).HasName("UIX_Task_NormalizedTitle").IsUnique();

            builder.ToTable(nameof(Task));
        }
    }
}