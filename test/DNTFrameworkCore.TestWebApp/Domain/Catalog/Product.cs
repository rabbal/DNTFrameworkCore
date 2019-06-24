using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Catalog
{
    public class Product : TrackableEntity<long>, IHasRowVersion, ICreationTracking, IModificationTracking,
        INumberedEntity
    {
        public const int MaxTitleLength = 50;

        public string Title { get; set; }
        public string Number { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }
    }
}