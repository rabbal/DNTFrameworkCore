using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Pending = new OrderStatus(1, nameof(Pending));
        public static readonly OrderStatus Processing = new OrderStatus(2, nameof(Processing));
        public static readonly OrderStatus Complete = new OrderStatus(3, nameof(Complete));
        public static readonly OrderStatus Cancelled = new OrderStatus(4, nameof(Cancelled));

        private OrderStatus(int id, string name) : base(id, name)
        {
        }
    }
}