using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Blogging
{
    public class Blog : Entity, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTitleLength = 50;
        public const int MaxUrlLength = 50;
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Url { get; set; }
        public byte[] Version { get; set; }
        public string Hash { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}