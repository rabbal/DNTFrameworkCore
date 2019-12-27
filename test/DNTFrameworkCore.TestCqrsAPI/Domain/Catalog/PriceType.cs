using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class PriceType : AggregateRoot, IHasRowVersion
    {
        private PriceType(Title title)
        {
            Title = title;
        }

        public static Result<PriceType> New(Title title, IPriceTypePolicy policy)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            var priceType = new PriceType(title);
            if (!policy.IsUnique(priceType)) return Fail<PriceType>("PriceType Title Should Be Unique");
                
            priceType.AddDomainEvent(new PriceTypeCreated(priceType));
            
            return Ok(priceType);
        }

        public Title Title { get; private set; }
    }
}