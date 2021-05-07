using System;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;
using static DNTFrameworkCore.Functional.Result;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class PriceType : Entity<long>, IAggregateRoot
    {
        private PriceType(Title title)
        {
            Title = title;
        }
        
        public PriceType(Title title, IPriceTypePolicy policy)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            Title = title;

            if (!policy.IsUnique(this)) ThrowDomainException("PriceType Title Should Be Unique");

            AddDomainEvent(new PriceTypeCreatedDomainEvent(this));
        }

        public Title Title { get; private set; }

        public static Result<PriceType> New(Title title, IPriceTypePolicy policy)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            var priceType = new PriceType(title);
            if (!policy.IsUnique(priceType)) return Fail<PriceType>("PriceType Title Should Be Unique");

            priceType.AddDomainEvent(new PriceTypeCreatedDomainEvent(priceType));

            return Ok(priceType);
        }
    }
}