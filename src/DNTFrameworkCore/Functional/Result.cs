using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Functional
{
    public class Result
    {
        private readonly List<ValidationFailure> _failures;
        private static readonly Result _ok = new Result(true, string.Empty);

        protected Result(bool succeeded, string message) : this(succeeded, message,
            Enumerable.Empty<ValidationFailure>())
        {
            Succeeded = succeeded;
            Message = message;
        }

        protected Result(bool succeeded, string message, IEnumerable<ValidationFailure> failures)
        {
            Succeeded = succeeded;
            Message = message;
            _failures = failures.ToList();
        }

        public bool Succeeded { get; }
        public string Message { get; }

        public IReadOnlyList<ValidationFailure> Failures => _failures.AsReadOnly();

        [DebuggerStepThrough]
        public static Result Ok()
        {
            return _ok;
        }

        [DebuggerStepThrough]
        public static Result Failed(string message)
        {
            return new Result(false, message);
        }

        [DebuggerStepThrough]
        public static Result Failed(string message, IEnumerable<ValidationFailure> failures)
        {
            return new Result(false, message, failures);
        }

        [DebuggerStepThrough]
        public static Result<T> Failed<T>(string message)
        {
            return new Result<T>(default, false, message);
        }

        [DebuggerStepThrough]
        public static Result<T> Failed<T>(string message, IEnumerable<ValidationFailure> failures)
        {
            return new Result<T>(default, false, message, failures);
        }

        [DebuggerStepThrough]
        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        [DebuggerStepThrough]
        public static Result Combine(string seperator, params Result[] results)
        {
            var failedResults = results.Where(x => !x.Succeeded).ToList();

            if (!failedResults.Any())
                return Ok();

            var message = string.Join(seperator, failedResults.Select(x => x.Message).ToArray());
            var failures = failedResults.SelectMany(r => r.Failures).ToList();

            return Failed(message, failures);
        }

        [DebuggerStepThrough]
        public static Result Combine(params Result[] results)
        {
            return Combine(", ", results);
        }

        [DebuggerStepThrough]
        public static Result Combine<T>(params Result<T>[] results)
        {
            return Combine(", ", results);
        }

        [DebuggerStepThrough]
        public static Result Combine<T>(string seperator, params Result<T>[] results)
        {
            var untyped = results.Select(result => (Result)result).ToArray();
            return Combine(seperator, untyped);
        }

        public override string ToString()
        {
            return Succeeded
                ? "Succeeded"
                : $"Failed : {Message}";
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        protected internal Result(T value, bool succeeded, string message)
            : base(succeeded, message)
        {
            _value = value;
        }

        protected internal Result(T value, bool succeeded, string message, IEnumerable<ValidationFailure> failures)
            : base(succeeded, message, failures)
        {
            _value = value;
        }

        public T Value
        {
            get
            {
                if (!Succeeded)
                    throw new InvalidOperationException("There is no value for failure.");

                return _value;
            }
        }
    }
}