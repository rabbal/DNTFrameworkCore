using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.TestCqrsAPI.Domain.Sales;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class Order : AggregateRoot<long>
    {
        private int _statusId;
        public OrderStatus Status { get; private set; }
        public SaleMethod SaleMethod { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public Address ShippingAddress { get; private set; }
        public DateTimeOffset DateTime { get; private set; }
        private List<OrderLine> _lines;
        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
        private List<OrderNote> _notes;
        public IReadOnlyCollection<OrderNote> Notes => _notes.AsReadOnly();

        private Order()
        {
            _lines = new List<OrderLine>();
            _notes = new List<OrderNote>();
        }

        public static Result<Order> Create(CustomerId customerId, Address address)
        {
            Guard.ArgumentNotNull(customerId, nameof(customerId));
            Guard.ArgumentNotNull(address, nameof(address));

            var order = new Order
            {
                _statusId = OrderStatus.Pending.Id,
                DateTime = DateTimeOffset.UtcNow,
                CustomerId = customerId,
                ShippingAddress = address
            };

            return Result.Ok(order);
        }

        public Result AddOrderLine(int productId, Price unitPrice, int quantity = 1, decimal discount = 0)
        {
            //todo: business rules
            if (_lines.Any(line => line.ProductId == productId))
            {
                
            }
            return OrderLine.Create(productId, unitPrice, quantity, discount)
                .OnSuccess(line => _lines.Add(line));
        }

        public Result AddOrderNote(string content)
        {
            return OrderNote.Create(content)
                .OnSuccess(note => _notes.Add(note));
        }

        public Result Cancel()
        {
            //todo: business rules
            _statusId = OrderStatus.Cancelled.Id;

            return Result.Ok();
        }

        public Result Complete()
        {
            //todo: business rules
            _statusId = OrderStatus.Complete.Id;

            return Result.Ok();
        }
    }
}