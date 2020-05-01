using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Catalog
{
    public class PriceType : Entity
    {
        public string Title { get; set; }
        public string NormalizedTitle { get; set; }
    }
}