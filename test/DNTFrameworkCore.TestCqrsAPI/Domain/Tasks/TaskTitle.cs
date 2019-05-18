using System.Collections.Generic;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Tasks
{
    public class TaskTitle : ValueObject
    {
        public string Value { get; }

        private TaskTitle() { }
        private TaskTitle(string value)
        {
            Value = value;
        }

        public static Result<TaskTitle> Create(string title)
        {
            title = title ?? string.Empty;

            if (title.Length == 0)
            {
                return Result.Failed<TaskTitle>("title should not be empty");
            }

            if (title.Length > 100)
            {
                return Result.Failed<TaskTitle>("title is too long");
            }

            return Result.Ok(new TaskTitle(title));
        }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static implicit operator string(TaskTitle title)
        {
            return title.Value;
        }

        public static explicit operator TaskTitle(string title)
        {
            return Create(title).Value;
        }
    }
}
