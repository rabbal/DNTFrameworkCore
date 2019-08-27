using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class Product : AggregateRoot, IHasRowVersion, INumberedEntity, ICreationTracking, IModificationTracking
    {
        private readonly List<ProductPrice> _prices = new List<ProductPrice>();

        private Product()
        {
        }

        private Product(Title title)
        {
            Title = title;
        }

        private ProductPrice DefaultPrice => _prices.Find(p => p.IsDefault);

        public Title Title { get; }
        public IReadOnlyList<ProductPrice> Prices => _prices.AsReadOnly();
        public string Number { get; private set; }

        public static Result<Product> New(Title title, IProductPolicy policy)
        {
            if (title == null) throw new ArgumentNullException(nameof(title));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            var product = new Product(title);
            if (!policy.IsUnique(product)) return Fail<Product>("product title must be unique");

            product.AddDomainEvent(new ProductCreated(product));

            return Ok(product);
        }

        public Result AddPrice(PriceType priceType, Price price)
        {
            if (priceType == null) throw new ArgumentNullException(nameof(priceType));
            if (price == null) throw new ArgumentNullException(nameof(price));

            if (_prices.Exists(p => p.PriceType == priceType))
                return Fail($"one price with priceType:{priceType.Title} already exists");

            var productPrice = new ProductPrice(this, priceType, price);
            _prices.Add(productPrice);

            return Ok();
        }

        public Result ChangeDefaultPrice(ProductPrice price)
        {
            if (price == DefaultPrice) return Ok();

            if (!_prices.Contains(price)) return Fail("price not found");

            DefaultPrice.UnmarkIsDefault();
            price.MarkIsDefault();

            return Ok();
        }

        public Result RemovePrice(ProductPrice price)
        {
            if (!_prices.Contains(price))
                return Fail($"price with priceType: {price.PriceType.Title} not found");

            if (price == DefaultPrice) return Fail("can not remove default price");

            _prices.Remove(price);

            return Ok();
        }

        public Price FindPrice(PriceType priceType)
        {
            var price = _prices.Find(p => p.PriceType == priceType);
            return price?.Price ?? DefaultPrice.Price;
        }
    }
}