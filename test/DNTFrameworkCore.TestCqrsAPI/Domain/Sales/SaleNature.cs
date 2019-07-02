using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public abstract class SaleNature : Enumeration
    {
        public static readonly SaleNature Restaurant = new RestaurantSaleNature();
        public static readonly SaleNature FastFood = new FastFoodSaleNature();
        public static readonly SaleNature Delivery = new DeliverySaleNature();

        private SaleNature() //Required for ORM
        {
        }

        private SaleNature(int value, string name) : base(value, name)
        {
        }

        public virtual bool ShipmentEnabled => false;
        public virtual bool TableRequired => false;
        public abstract IReadOnlyList<Action> Commands { get; }
        public abstract IReadOnlyList<State> States { get; }
        public abstract IReadOnlyList<Transition> Transitions { get; }

        public static IReadOnlyList<SaleNature> List() =>
            new[] {Restaurant, FastFood, Delivery};

        public static explicit operator int(SaleNature nature) => nature.Value;
        public static explicit operator SaleNature(int value) => FromValue(value);
        public static explicit operator SaleNature(string name) => FromName(name);

        public static SaleNature FromName(string name)
        {
            var nature = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (nature == null)
            {
                throw new InvalidOperationException(
                    $"Possible values for SaleMethodNature: {string.Join(",", List().Select(s => s.Name))}");
            }

            return nature;
        }

        public static SaleNature FromValue(int value)
        {
            var nature = List().SingleOrDefault(s => s.Value == value);

            if (nature == null)
            {
                throw new InvalidOperationException(
                    $"Possible values for SaleMethodNature: {string.Join(",", List().Select(s => s.Value))}");
            }

            return nature;
        }

        private class RestaurantSaleNature : SaleNature
        {
            public RestaurantSaleNature() : base(1, nameof(Restaurant))
            {
            }

            public override bool TableRequired => true;
            public override IReadOnlyList<Action> Commands { get; }
            public override IReadOnlyList<State> States { get; }
            public override IReadOnlyList<Transition> Transitions { get; }
        }

        private class FastFoodSaleNature : SaleNature
        {
            public FastFoodSaleNature() : base(2, nameof(FastFood))
            {
            }


            public override IReadOnlyList<Action> Commands { get; }
            public override IReadOnlyList<State> States { get; }
            public override IReadOnlyList<Transition> Transitions { get; }
        }

        private class DeliverySaleNature : SaleNature
        {
            private readonly IList<Action> _commands;

            public DeliverySaleNature() : base(3, nameof(Delivery))
            {
//                _commands = new List<Action>
//                {
//                    new Action("Save", ActionType.Save),
//                    new Action("Print For Kitchen", ActionType.Print),
//                    new Action("Print For Customer", ActionType.Print),
//                    new Action("Ship", ActionType.Ship)
//                };
            }

            public override bool ShipmentEnabled => true;

            public override IReadOnlyList<Action> Commands { get; }
            public override IReadOnlyList<State> States { get; }
            public override IReadOnlyList<Transition> Transitions { get; }
        }
    }
}