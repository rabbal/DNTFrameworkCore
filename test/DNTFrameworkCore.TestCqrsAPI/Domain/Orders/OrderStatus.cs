using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Pending = new OrderStatus(1, nameof(Pending));
        public static readonly OrderStatus Preparation = new OrderStatus(2, nameof(Preparation));
        public static readonly OrderStatus Paid = new OrderStatus(3, nameof(Paid));
        public static readonly OrderStatus Shipped = new OrderStatus(4, nameof(Shipped));
        public static readonly OrderStatus Completed = new OrderStatus(5, nameof(Completed));
        public static readonly OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled));

        private OrderStatus() //Required for ORM
        {
        }

        private OrderStatus(int value, string name) : base(value, name)
        {
        }

        public static IReadOnlyList<OrderStatus> List() =>
            new[] {Pending, Preparation, Paid, Shipped, Completed, Cancelled};

        public static explicit operator int(OrderStatus nature) => nature.Value;
        public static explicit operator OrderStatus(int id) => FromValue<OrderStatus>(id);

        public static OrderStatus FromName(string name)
        {
            var status = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (status == null)
            {
                throw new InvalidOperationException(
                    $"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return status;
        }

        public static OrderStatus FromValue(int value)
        {
            var status = List().SingleOrDefault(s => s.Value == value);

            if (status == null)
            {
                throw new InvalidOperationException(
                    $"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Value))}");
            }

            return status;
        }
    }
}