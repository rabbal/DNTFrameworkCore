using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel
{
    public class Title : ValueObject
    {
        public string Value { get; }

        private Title() { }
        private Title(string value)
        {
            Value = value;
        }

        public static Result<Title> Create(string title)
        {
            title = title ?? string.Empty;

            if (title.Length == 0)
            {
                return Fail<Title>("title should not be empty");
            }

            if (title.Length > 100)
            {
                return Fail<Title>("title is too long");
            }

            return Ok(new Title(title));
        }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static implicit operator string(Title title)
        {
            return title.Value;
        }

        public static explicit operator Title(string title)
        {
            return Create(title).Value;
        }
    }
}
