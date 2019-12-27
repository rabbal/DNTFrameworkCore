using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel
{
    public class Title : ValueObject
    {
        private Title()
        {
        }

        private Title(string value)
        {
            Value = value;
        }

        public string Value { get; }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static Result<Title> New(string title)
        {
            title = title ?? string.Empty;

            if (title.Length == 0) return Fail<Title>("title should not be empty");

            return title.Length > 100 ? Fail<Title>("title is too long") : Ok(new Title(title));
        }

        public static implicit operator string(Title title)
        {
            return title.Value;
        }

        public static explicit operator Title(string title)
        {
            return New(title).Value;
        }
    }
}