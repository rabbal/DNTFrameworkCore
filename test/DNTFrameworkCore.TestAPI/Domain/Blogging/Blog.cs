using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.TestAPI.Domain.Blogging
{
    public class Blog : TrackableEntity, IAggregateRoot
    {
        public const int MaxTitleLength = 50;
        public const int MaxUrlLength = 50;
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Url { get; set; }
        public byte[] RowVersion { get; set; }
    }
}