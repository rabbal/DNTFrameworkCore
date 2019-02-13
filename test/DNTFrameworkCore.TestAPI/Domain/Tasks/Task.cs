using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.TestAPI.Domain.Tasks
{
    public class Task : TrackableEntity<int>, IAggregateRoot, INumberedEntity
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