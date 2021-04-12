namespace DNTFrameworkCore.Querying
{
    public sealed class FilteringOperator
    {
        private readonly string _value;

        public static readonly FilteringOperator IsEqualTo = new("eq");
        public static readonly FilteringOperator IsEqualToField = new("eqf");
        public static readonly FilteringOperator IsNotEqualTo = new("neq");
        public static readonly FilteringOperator IsNotEqualToField = new("neqf");
        public static readonly FilteringOperator IsLessThan = new("lt");
        public static readonly FilteringOperator IsLessThanField = new("ltf");
        public static readonly FilteringOperator IsLessThanOrEqualTo = new("lte");
        public static readonly FilteringOperator IsLessThanOrEqualToField = new("ltef");
        public static readonly FilteringOperator IsGreaterThan = new("gt");
        public static readonly FilteringOperator IsGreaterThanField = new("gtf");
        public static readonly FilteringOperator IsGreaterThanOrEqualTo = new("gte");
        public static readonly FilteringOperator IsGreaterThanOrEqualToField = new("gtef");

        public FilteringOperator(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }
    }
}