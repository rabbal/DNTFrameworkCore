using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Functional
{
    public class Result
    {
        private static readonly Result _ok = new Result(false, string.Empty);
        private readonly List<ValidationFailure> _failures;

        protected Result(bool failed, string message) : this(failed, message,
            Enumerable.Empty<ValidationFailure>())
        {
            Failed = failed;
            Message = message;
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

        [DebuggerStepThrough]
        public static Result Ok() => _ok;

        [DebuggerStepThrough]
        public static Result Fail(string message)
        {
            return new Result(true, message);
        }

        [DebuggerStepThrough]
        public static Result Fail(string message, IEnumerable<ValidationFailure> failures) =>
            new Result(true, message, failures);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string message) => new Result<T>(default, true, message);

        [DebuggerStepThrough]
        public static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures) =>
            new Result<T>(default, true, message, failures);

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value) => new Result<T>(value, false, string.Empty);

        [DebuggerStepThrough]
        public static Result Combine(string seperator, params Result[] results)
        {
            var failedList = results.Where(x => !x.Failed).ToList();

            if (!failedList.Any()) return Ok();

            var message = string.Join(seperator, failedList.Select(x => x.Message).ToArray());
            var failures = failedList.SelectMany(r => r.Failures).ToList();

            return Fail(message, failures);
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results) => Combine(", ", results);

        [DebuggerStepThrough]
        public static Result Combine<T>(string seperator, params Result<T>[] results)
        {
            var untyped = results.Select(result => (Result) result).ToArray();
            return Combine(seperator, untyped);
        }

        public override string ToString()
        {
            return Failed
                ? "Ok"
                : $"Failed : {Message}";
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