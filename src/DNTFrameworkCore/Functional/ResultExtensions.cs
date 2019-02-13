using System;

namespace DNTFrameworkCore.Functional
{
    public static class ResultExtensions
    {
        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<T, TK> func)
        {
            return !result.Succeeded ? Result.Failed<TK>(result.Message) : Result.Ok(func(result.Value));
        }
   
        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string message)
        {
            if (!result.Succeeded)
                return Result.Failed<T>(result.Message);

            return !predicate(result.Value) ? Result.Failed<T>(message) : Result.Ok(result.Value);
        }

        public static Result<TK> Map<T, TK>(this Result<T> result, Func<T, TK> func)
        {
            return !result.Succeeded ? Result.Failed<TK>(result.Message) : Result.Ok(func(result.Value));
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (result.Succeeded) action(result.Value);

            return result;
        }

        public static T OnBoth<T>(this Result result, Func<Result, T> func)
        {
            return func(result);
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (result.Succeeded) action();

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<T> func)
        {
            return !result.Succeeded ? Result.Failed<T>(result.Message) : Result.Ok(func());
        }

        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<T, Result<TK>> func)
        {
            return !result.Succeeded ? Result.Failed<TK>(result.Message) : func(result.Value);
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<Result<T>> func)
        {
            return !result.Succeeded ? Result.Failed<T>(result.Message) : func();
        }

        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<Result<TK>> func)
        {
            return !result.Succeeded ? Result.Failed<TK>(result.Message) : func();
        }

        public static Result OnSuccess<T>(this Result<T> result, Func<T, Result> func)
        {
            return !result.Succeeded ? Result.Failed(result.Message) : func(result.Value);
        }

        public static Result OnSuccess(this Result result, Func<Result> func)
        {
            return !result.Succeeded ? result : func();
        }

        public static Result Ensure(this Result result, Func<bool> predicate, string message)
        {
            if (!result.Succeeded)
                return Result.Failed(result.Message);

            return !predicate() ? Result.Failed(message) : Result.Ok();
        }

        public static Result<T> Map<T>(this Result result, Func<T> func)
        {
            return !result.Succeeded ? Result.Failed<T>(result.Message) : Result.Ok(func());
        }


        public static TK OnBoth<T, TK>(this Result<T> result, Func<Result<T>, TK> func)
        {
            return func(result);
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action action)
        {
            if (!result.Succeeded) action();

            return result;
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (!result.Succeeded) action();

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
        {
            if (!result.Succeeded) action(result.Message);

            return result;
        }

        public static Result OnFailure(this Result result, Action<string> action)
        {
            if (!result.Succeeded) action(result.Message);

            return result;
        }
    }
}