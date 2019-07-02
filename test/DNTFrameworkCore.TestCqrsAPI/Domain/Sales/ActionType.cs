using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class ActionType : Enumeration
    {
        public static readonly ActionType Save = new ActionType(1, nameof(Save));
        public static readonly ActionType Print = new ActionType(3, nameof(Print));
        public static readonly ActionType Ship = new ActionType(4, nameof(Ship));
        public static readonly ActionType Pay = new ActionType(5, nameof(Pay));
        public static readonly ActionType Cancel = new ActionType(6, nameof(Cancel));

        private ActionType(int value, string name) : base(value, name)
        {
        }
    }
}