using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class PriceType : TrackableEntity, IHasRowVersion
    {
        public string Title { get; set; }
        public byte[] RowVersion { get; set; }
    }
}