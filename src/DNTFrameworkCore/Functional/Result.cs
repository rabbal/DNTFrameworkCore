using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Functional
{
    public class Result
    {
        private static readonly Result _ok = new(false, string.Empty);
        private readonly List<ValidationFailure> _failures;

        public static readonly Result None = new(false, string.Empty);
        public static readonly Task<Result> NoneTask = Task.FromResult(None);

        protected Result(bool failed, string message) : this(failed, message,
            Enumerable.Empty<ValidationFailure>())
        {
        }

        protected Result(bool failed, string message, IEnumerable<ValidationFailure> failures)
        {
            Failed = failed;
            Message = message;
            _failures = failures.ToList();
        }

        public bool Failed { get; }
        public string Message { get; }
        public IEnumerable<ValidationFailure> Failures => _failures.AsReadOnly();

        public Result WithFailure(string memberName, string message)
        {
            if (!Failed) throw new InvalidOperationException("Can not add failure to ok result!");

            _failures.Add(new ValidationFailure(memberName, message));
            return this;
        }

        public static Result Ok() => _ok;

        public static Result Ok(string message)
        {
            return new(false, message);
        }

        public static Result Fail(string message)
        {
            return new(true, message);
        }

        public static Result Fail(string message, IEnumerable<ValidationFailure> failures)
        {
            return new(true, message, failures);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new(default, true, message);
        }

        public static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures)
        {
            return new(default, true, message, failures);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new(value, false, string.Empty);
        }

        public static Result<T> Ok<T>(string message, T value)
        {
            return new(value, false, message);
        }

        public static Result Combine(string symbol, params Result[] results)
        {
            var failedList = results.Where(x => !x.Failed).ToList();

            if (!failedList.Any()) return Ok();

            var message = string.Join(symbol, failedList.Select(x => x.Message).ToArray());
            var failures = failedList.SelectMany(r => r.Failures).ToList();

            return Fail(message, failures);
        }

        public static Result Combine(params Result[] results)
        {
            return Combine(", ", results);
        }

        public static Result Combine<T>(params Result<T>[] results)
        {
            return Combine(", ", results);
        }

        public static Result Combine<T>(string symbol, params Result<T>[] results)
        {
            var untyped = results.Select(result => (Result) result).ToArray();
            return Combine(symbol, untyped);
        }

        public override string ToString()
        {
            return !Failed
                ? "Ok"
                : $"Failed: {Message}";
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        protected internal Result(T value, bool failed, string message)
            : base(failed, message)
        {
            _value = value;
        }

        protected internal Result(T value, bool failed, string message, IEnumerable<ValidationFailure> failures)
            : base(failed, message, failures)
        {
            _value = value;
        }

        public T Value => !Failed ? _value : throw new InvalidOperationException("There is no value for failure.");
    }
}