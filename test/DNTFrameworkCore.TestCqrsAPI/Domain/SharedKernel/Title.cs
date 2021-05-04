using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel
{
    public class Title : ValueObject
    {
        private Title()
        {
        }

        public Title(string value)
        {
            value ??= string.Empty;

            if (value.Length == 0) throw new BusinessRuleException("title should not be empty");

            if (value.Length > 100) throw new BusinessRuleException("title is too long");
        }

        public string Value { get; private set; }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static Result<Title> New(string value)
        {
            value ??= string.Empty;

            if (value.Length == 0) return Fail<Title>("title should not be empty");

            return value.Length > 100 ? Fail<Title>("title is too long") : Ok(new Title { Value = value });
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