using System;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class OrderLine : Entity<long>
    {
        public decimal Discount { get; private set; }
        public int Quantity { get; private set; }
        public Price UnitPrice { get; private set; }
        public Price Price { get; private set; }
        public int ProductId { get; private set; }

        private OrderLine()
        {
        }

        public static Result<OrderLine> Create(int productId, Price unitPrice, int quantity = 1, decimal discount = 0)
        {
            //todo:input validation or business rule validation

            var line = new OrderLine
            {
                ProductId = productId,
                UnitPrice = unitPrice,
                Quantity = quantity,
                Discount = discount,
            };

            var price = Price.Create(unitPrice.Value * quantity, unitPrice.Currency);
            if (price.Failed) return Result.Fail<OrderLine>(price.Message, price.Failures);

            line.Price = price.Value;
            
            return Result.Ok(line);
        }

        public Result ApplyDiscount(decimal discount)
        {
            if (discount > Price.Value)
                return Result.Fail("discount should not  ");

            Discount = discount;
            
            return Result.Ok();
        }
    }
}