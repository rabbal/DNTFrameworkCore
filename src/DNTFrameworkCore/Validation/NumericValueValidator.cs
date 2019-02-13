using System;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.Validation
{
    [Serializable]
    [Validator("NUMERIC")]
    public class NumericValueValidator : ValueValidatorBase
    {
        public int MinValue
        {
            get => (this["MinValue"] ?? "0").To<int>();
            set => this["MinValue"] = value;
        }

        public int MaxValue
        {
            get => (this["MaxValue"] ?? "0").To<int>();
            set => this["MaxValue"] = value;
        }

        public NumericValueValidator()
        {

        }

        public NumericValueValidator(int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is int i)
            {
                return IsValidInternal(i);
            }

            if (!(value is string s)) return false;
            
            return int.TryParse(s, out var intValue) && IsValidInternal(intValue);
        }

        protected virtual bool IsValidInternal(int value)
        {
            return value.IsBetween(MinValue, MaxValue);
        }
    }
}