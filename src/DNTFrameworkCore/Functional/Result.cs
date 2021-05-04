using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Functional
{
    public class Result
    {
        private static readonly Result _ok = new(false, string.Empty, string.Empty);
        private readonly List<ValidationFailure> _failures;

        public static readonly Result None = new(false, string.Empty, string.Empty);
        public static readonly Task<Result> NoneTask = Task.FromResult(None);

        protected Result(bool failed, string message, string details) : this(failed, message, details,
            Enumerable.Empty<ValidationFailure>())
        {
        }

        protected Result(bool failed, string message, string details, IEnumerable<ValidationFailure> failures)
        {
            Failed = failed;
            Message = message;
            Details = details;

            _failures = failures.ToList();
        }

        public bool IsNone => this == None;
        public bool Failed { get; }
        public string Message { get; }
        public string Details { get; }
        public IEnumerable<ValidationFailure> Failures => _failures.AsReadOnly();
        public Result WithFailure(string memberName, string message)
        {
            if (!Failed) throw new InvalidOperationException("Can not add failure to ok result!");

            _failures.Add(new ValidationFailure(memberName, message));
            return this;
        }

        public static Result Ok() => _ok;

        public static Result Ok(string message, string details = null)
        {
            return new(false, message, details);
        }

        public static Result Fail(string message, string details = null)
        {
            return new(true, message, details);
        }


        public static Result Fail(string message, IEnumerable<ValidationFailure> failures)
        {
            return new(true, message, string.Empty, failures);
        }

        public static Result<T> Fail<T>(string message, string details = null)
        {
            return new(default, true, message, details);
        }

        public static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures)
        {
            return new(default, true, message, string.Empty, failures);
        }

        public static Result<T> Fail<T>(string message, string details, IEnumerable<ValidationFailure> failures)
        {
            return new(default, true, message, details, failures);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new(value, false, string.Empty, string.Empty);
        }

        public static Result<T> Ok<T>(T value, string message, string details = null)
        {
            return new(value, false, message, details);
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
            var untyped = results.Select(result => (Result)result).ToArray();
            return Combine(symbol, untyped);
        }

        public override string ToString()
        {
            return !Failed
                ? $"Ok"
                : $"Failed: {Message}";
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;
        public static new readonly Result<T> None = new(default, false, string.Empty, string.Empty);
        public static new readonly Task<Result<T>> NoneTask = Task.FromResult(None);
        protected internal Result(T value, bool failed, string message, string details)
            : base(failed, message, details)
        {
            _value = value;
        }

        protected internal Result(T value, bool failed, string message, string details, IEnumerable<ValidationFailure> failures)
            : base(failed, message, details, failures)
        {
            _value = value;
        }

        public new bool IsNone => this == None;
        public T Value => !Failed ? _value : throw new InvalidOperationException("There is no value for failure.");

        public static implicit operator Result<T>(T value)
        {
            return Ok(value);
        }
    }
}