using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;
using static DNTFrameworkCore.Functional.Result;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class Product : Entity<long>, IAggregateRoot, INumberedEntity
    {
        private readonly List<ProductPrice> _prices = new();

        protected Product() //ORM
        {
        }

        private Product(Title title) : this()
        {
            Title = title;
        }

        private ProductPrice Price => _prices.Find(p => p.IsDefault);
        public Title Title { get; }
        public IReadOnlyList<ProductPrice> Prices => _prices.AsReadOnly();

        public static Result<Product> New(Title title, IProductPolicy policy)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            var product = new Product(title);
            if (!policy.IsUnique(product)) return Fail<Product>("Product Title Should Be Unique");

            product.AddDomainEvent(new ProductCreatedDomainEvent(product));

            return Ok(product);
        }

        public Result<ProductPrice> AddPrice(PriceType priceType, Price price)
        {
            if (priceType == null) throw new ArgumentNullException(nameof(priceType));
            if (price == null) throw new ArgumentNullException(nameof(price));

            if (_prices.Exists(p => p.PriceType == priceType))
                return Fail<ProductPrice>($"A Price With PriceType:{priceType.Title} Already Exists");

            var productPrice = new ProductPrice(this, priceType, price);
            _prices.Add(productPrice);

            return Ok(productPrice);
        }

        public Result ChangeDefaultPrice(ProductPrice price)
        {
            if (price == Price) return Ok();

            if (!_prices.Contains(price)) return Fail("Price Not Found");

            Price.UnmarkIsDefault();
            price.MarkIsDefault();

            return Ok();
        }

        public Result RemovePrice(ProductPrice price)
        {
            if (!_prices.Contains(price))
                return Fail($"Price With PriceType: {price.PriceType.Title} Not Found");

            if (price == Price) return Fail("Can Not Remove Default Price");

            _prices.Remove(price);

            return Ok();
        }

        public Price FindPrice(PriceType priceType)
        {
            var price = _prices.Find(p => p.PriceType == priceType);
            return price?.Price ?? Price.Price;
        }

        public string Number { get; private set; }
    }
}