using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class StateType : Enumeration
    {
        public static readonly StateType Start = new StateType(1, nameof(Start));
        public static readonly StateType Normal = new StateType(2, nameof(Normal));
        public static readonly StateType Completed = new StateType(3, nameof(Completed));
        public static readonly StateType Cancelled = new StateType(4, nameof(Cancelled));

        private StateType() //Required for ORM
        {
        }

        private StateType(int value, string name) : base(value, name)
        {
        }

        public static IReadOnlyList<StateType> List() =>
            new[] {Start, Normal, Completed, Cancelled};

        public static explicit operator int(StateType nature) => nature.Value;
        public static explicit operator StateType(int id) => FromValue<StateType>(id);

        public static StateType FromName(string name)
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

        public static StateType FromValue(int value)
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