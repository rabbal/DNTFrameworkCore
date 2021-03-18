using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Blogging
{
    public class Blog : Entity, IHasRowVersion, ICreationTracking, IModificationTracking
    {
        public const int MaxTitleLength = 50;
        public const int MaxUrlLength = 50;
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
        public string Url { get; set; }
        public byte[] LogoImage { get; set; }
        public byte[] Version { get; set; }
    }
}