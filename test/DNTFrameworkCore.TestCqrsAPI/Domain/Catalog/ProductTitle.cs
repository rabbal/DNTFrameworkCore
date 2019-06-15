using System.Collections.Generic;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class ProductTitle : ValueObject
    {
        public string Value { get; }

        private ProductTitle() { }
        private ProductTitle(string value)
        {
            Value = value;
        }

        public static Result<ProductTitle> Create(string title)
        {
            title = title ?? string.Empty;

            if (title.Length == 0)
            {
                return Result.Fail<ProductTitle>("title should not be empty");
            }

            if (title.Length > 100)
            {
                return Result.Fail<ProductTitle>("title is too long");
            }

            return Result.Ok(new ProductTitle(title));
        }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static implicit operator string(ProductTitle title)
        {
            return title.Value;
        }

        public static explicit operator ProductTitle(string title)
        {
            return Create(title).Value;
        }
    }
}
