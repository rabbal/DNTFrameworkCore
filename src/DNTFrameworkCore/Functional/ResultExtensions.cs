using System;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Functional
{
    public static class ResultExtensions
    {
        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<T, TK> func)
        {
            return result.Failed ? Result.Fail<TK>(result.Message, result.Failures) : Result.Ok(func(result.Value));
        }

        public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, string message)
        {
            if (result.Failed) return result;

            return !predicate(result.Value) ? Result.Fail<T>(message) : Result.Ok(result.Value);
        }

        public static Result<TK> Map<T, TK>(this Result<T> result, Func<T, TK> func)
        {
            return result.Failed ? Result.Fail<TK>(result.Message, result.Failures) : Result.Ok(func(result.Value));
        }

        public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
        {
            if (!result.Failed) action(result.Value);

            return result;
        }

        public static T OnBoth<T>(this Result result, Func<Result, T> func)
        {
            return func(result);
        }

        public static Result OnSuccess(this Result result, Action action)
        {
            if (!result.Failed) action();

            return result;
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<T> func)
        {
            return result.Failed ? Result.Fail<T>(result.Message, result.Failures) : Result.Ok(func());
        }

        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<T, Result<TK>> func)
        {
            return result.Failed ? Result.Fail<TK>(result.Message, result.Failures) : func(result.Value);
        }

        public static Result<T> OnSuccess<T>(this Result result, Func<Result<T>> func)
        {
            return result.Failed ? Result.Fail<T>(result.Message, result.Failures) : func();
        }

        public static Result<TK> OnSuccess<T, TK>(this Result<T> result, Func<Result<TK>> func)
        {
            return result.Failed ? Result.Fail<TK>(result.Message, result.Failures) : func();
        }

        public static Result OnSuccess<T>(this Result<T> result, Func<T, Result> func)
        {
            return result.Failed ? Result.Fail(result.Message, result.Failures) : func(result.Value);
        }

        public static Result OnSuccess(this Result result, Func<Result> func)
        {
            return result.Failed ? result : func();
        }

        public static Result Ensure(this Result result, Func<bool> predicate, string message)
        {
            if (result.Failed) return result;

            return !predicate() ? Result.Fail(message) : Result.Ok();
        }

        public static Result<T> Map<T>(this Result result, Func<T> func)
        {
            return result.Failed ? Result.Fail<T>(result.Message, result.Failures) : Result.Ok(func());
        }

        public static TK OnBoth<T, TK>(this Result<T> result, Func<Result<T>, TK> func)
        {
            return func(result);
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action action)
        {
            if (result.Failed) action();

            return result;
        }

        public static Result OnFailure(this Result result, Action action)
        {
            if (result.Failed) action();

            return result;
        }

        public static Result<T> OnFailure<T>(this Result<T> result, Action<string> action)
        {
            if (result.Failed) action(result.Message);

            return result;
        }

        public static Result OnFailure(this Result result, Action<string> action)
        {
            if (result.Failed) action(result.Message);

            return result;
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask, Func<T, Task<TK>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            var value = await func(result.Value);

            return Result.Ok(value);
        }

        public static async Task<Result<T>> Ensure<T>(this Task<Result<T>> resultTask, Func<T, Task<bool>> predicate,
            string message)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed)
                return result;

            if (!await predicate(result.Value).ConfigureAwait(false))
                return Result.Fail<T>(message);

            return result;
        }

        public static Task<Result<TK>> Map<T, TK>(this Task<Result<T>> resultTask, Func<T, Task<TK>> func)
        {
            return resultTask.OnSuccess(func);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result<T>> resultTask, Func<T, Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.Failed) await action(result.Value).ConfigureAwait(false);

            return result;
        }

        public static async Task<T> OnBoth<T>(this Task<Result> resultTask, Func<Result, Task<T>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return await func(result).ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess(this Task<Result> resultTask, Func<Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (!result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result> resultTask, Func<Task<T>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail<T>(result.Message, result.Failures);

            var value = await func().ConfigureAwait(false);

            return Result.Ok(value);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask,
            Func<T, Task<Result<TK>>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            return await func(result.Value).ConfigureAwait(false);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result> resultTask, Func<Task<Result<T>>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail<T>(result.Message, result.Failures);

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask,
            Func<Task<Result<TK>>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess<T>(this Task<Result<T>> resultTask, Func<T, Task<Result>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return Result.Fail(result.Message, result.Failures);

            return await func(result.Value).ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess(this Task<Result> resultTask, Func<Task<Result>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return result;

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result> Ensure(this Task<Result> resultTask, Func<Task<bool>> predicate,
            string message)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) return result;

            return !await predicate() ? Result.Fail(message) : Result.Ok();
        }

        public static Task<Result<T>> Map<T>(this Task<Result> result, Func<Task<T>> func)
        {
            return result.OnSuccess(func);
        }

        public static async Task<TK> OnBoth<T, TK>(this Task<Result<T>> resultTask, Func<Result<T>, Task<TK>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return await func(result).ConfigureAwait(false);
        }

        public static async Task<Result<T>> OnFailure<T>(this Task<Result<T>> resultTask, Func<Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result> OnFailure(this Task<Result> resultTask, Func<Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result<T>> OnFailure<T>(this Task<Result<T>> resultTask, Func<string, Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) await action(result.Message).ConfigureAwait(false);

            return result;
        }

        public static async Task<Result> OnFailure(this Task<Result> resultTask, Func<string, Task> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            if (result.Failed) await action(result.Message).ConfigureAwait(false);

            return result;
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask, Func<T, TK> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result<T>> Ensure<T>(this Task<Result<T>> resultTask, Func<T, bool> predicate,
            string message)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.Ensure(predicate, message);
        }

        public static Task<Result<TK>> Map<T, TK>(this Task<Result<T>> resultTask, Func<T, TK> func)
        {
            return resultTask.OnSuccess(func);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result<T>> resultTask, Action<T> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(action);
        }

        public static async Task<T> OnBoth<T>(this Task<Result> resultTask, Func<Result, T> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnBoth(func);
        }

        public static async Task<Result> OnSuccess(this Task<Result> resultTask, Action action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(action);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result> resultTask, Func<T> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask, Func<T, Result<TK>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Task<Result> resultTask, Func<Result<T>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Task<Result<T>> resultTask, Func<Result<TK>> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result> OnSuccess<T>(this Task<Result<T>> resultTask, Func<T, Result> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result> OnSuccess(this Task<Result> resultTask, Func<Result> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnSuccess(func);
        }

        public static async Task<Result> Ensure(this Task<Result> resultTask, Func<bool> predicate, string message)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.Ensure(predicate, message);
        }

        public static Task<Result<T>> Map<T>(this Task<Result> resultTask, Func<T> func)
        {
            return resultTask.OnSuccess(func);
        }

        public static async Task<TK> OnBoth<T, TK>(this Task<Result<T>> resultTask, Func<Result<T>, TK> func)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnBoth(func);
        }

        public static async Task<Result<T>> OnFailure<T>(this Task<Result<T>> resultTask, Action action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnFailure(action);
        }

        public static async Task<Result> OnFailure(this Task<Result> resultTask, Action action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnFailure(action);
        }

        public static async Task<Result<T>> OnFailure<T>(this Task<Result<T>> resultTask, Action<string> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnFailure(action);
        }

        public static async Task<Result> OnFailure(this Task<Result> resultTask, Action<string> action)
        {
            var result = await resultTask.ConfigureAwait(false);

            return result.OnFailure(action);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Result<T> result, Func<T, Task<TK>> func)
        {
            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            var value = await func(result.Value);

            return Result.Ok(value);
        }

        public static async Task<Result<T>> Ensure<T>(this Result<T> result, Func<T, Task<bool>> predicate,
            string message)
        {
            if (result.Failed)
                return result;

            if (!await predicate(result.Value).ConfigureAwait(false))
                return Result.Fail<T>(message);

            return result;
        }

        public static Task<Result<TK>> Map<T, TK>(this Result<T> result, Func<T, Task<TK>> func)
        {
            return result.OnSuccess(func);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Result<T> result, Func<T, Task> action)
        {
            if (!result.Failed) await action(result.Value).ConfigureAwait(false);

            return result;
        }

        public static async Task<T> OnBoth<T>(this Result result, Func<Result, Task<T>> func)
        {
            return await func(result).ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess(this Result result, Func<Task> action)
        {
            if (!result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result<T>> OnSuccess<T>(this Result result, Func<Task<T>> func)
        {
            if (result.Failed) return Result.Fail<T>(result.Message, result.Failures);

            var value = await func().ConfigureAwait(false);

            return Result.Ok(value);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Result<T> result, Func<T, Task<Result<TK>>> func)
        {
            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            return await func(result.Value).ConfigureAwait(false);
        }

        public static async Task<Result<T>> OnSuccess<T>(this Result result, Func<Task<Result<T>>> func)
        {
            if (result.Failed) return Result.Fail<T>(result.Message, result.Failures);

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result<TK>> OnSuccess<T, TK>(this Result<T> result, Func<Task<Result<TK>>> func)
        {
            if (result.Failed) return Result.Fail<TK>(result.Message, result.Failures);

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess<T>(this Result<T> result, Func<T, Task<Result>> func)
        {
            if (result.Failed) return Result.Fail(result.Message, result.Failures);

            return await func(result.Value).ConfigureAwait(false);
        }

        public static async Task<Result> OnSuccess(this Result result, Func<Task<Result>> func)
        {
            if (result.Failed) return result;

            return await func().ConfigureAwait(false);
        }

        public static async Task<Result> Ensure(this Result result, Func<Task<bool>> predicate, string message)
        {
            if (result.Failed) return result;

            return !await predicate() ? Result.Fail(message) : Result.Ok();
        }

        public static Task<Result<T>> Map<T>(this Result result, Func<Task<T>> func)
        {
            return result.OnSuccess(func);
        }

        public static async Task<TK> OnBoth<T, TK>(this Result<T> result, Func<Result<T>, Task<TK>> func)
        {
            return await func(result).ConfigureAwait(false);
        }

        public static async Task<Result<T>> OnFailure<T>(this Result<T> result, Func<Task> action)
        {
            if (result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result> OnFailure(this Result result, Func<Task> action)
        {
            if (result.Failed) await action().ConfigureAwait(false);

            return result;
        }

        public static async Task<Result<T>> OnFailure<T>(this Result<T> result, Func<string, Task> action)
        {
            if (result.Failed) await action(result.Message).ConfigureAwait(false);

            return result;
        }

        public static async Task<Result> OnFailure(this Result result, Func<string, Task> action)
        {
            if (result.Failed) await action(result.Message).ConfigureAwait(false);

            return result;
        }
    }
}