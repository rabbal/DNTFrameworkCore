using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.Orders;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class Transition : Entity
    {
        public Transition(Action trigger, State @from, State to,
            IEnumerable<ActivityType> activities)
        {
            Trigger = trigger;
            From = @from;
            To = to;
            Activities = activities.ToList();
        }

        public Action Trigger { get; private set; }
        public State From { get; private set; }
        public State To { get; private set; }
        public IReadOnlyList<ActivityType> Activities { get; private set; }
    }
}