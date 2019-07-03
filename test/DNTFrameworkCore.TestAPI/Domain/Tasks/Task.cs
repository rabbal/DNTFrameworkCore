using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Numbering;

namespace DNTFrameworkCore.TestAPI.Domain.Tasks
{
    [NumberedEntityOption(Start = 1, IncrementBy = 10, ResetFieldName = nameof(Task.Title))]
    public class Task : TrackableEntity, IHasRowVersion, ICreationTracking, IModificationTracking, INumberedEntity,
        ITenantEntity
    {
        public const int MaxTitleLength = 256;
        public const int MaxDescriptionLength = 1024;

        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Number { get; set; }
        public string Description { get; set; }
        public TaskState State { get; set; } = TaskState.Todo;
        public byte[] RowVersion { get; set; }
    }
}