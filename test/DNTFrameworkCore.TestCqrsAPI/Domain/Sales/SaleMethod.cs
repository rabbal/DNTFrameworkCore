using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Events;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethod : AggregateRoot, ICreationTracking, IModificationTracking, IHasRowVersion
    {
        public Title Title { get; private set; }
        public SaleNature Nature { get; private set; }

        private readonly List<SaleMethodProduct> _products;
        public IReadOnlyCollection<SaleMethodProduct> Products => _products.AsReadOnly();
        private readonly List<SaleMethodPriceType> _priceTypes;
        public IReadOnlyCollection<SaleMethodPriceType> PriceTypes => _priceTypes.AsReadOnly();

        private SaleMethod()
        {
            _products = new List<SaleMethodProduct>();
            _priceTypes = new List<SaleMethodPriceType>();
        }

        public static Result<SaleMethod> Create(Title title, SaleNature nature)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (nature == null) throw new ArgumentNullException(nameof(nature));

            var saleMethod = new SaleMethod
            {
                Nature = nature,
                Title = title
            };

            saleMethod.AddDomainEvent(new SaleMethodCreated(saleMethod.Id));

            return Ok(saleMethod);
        }
    }
}