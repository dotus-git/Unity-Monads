using System;

namespace Monads
{
    public static class ResultExtensions
    {
        [GarbageFree]
        public static Result ToResult(
            this bool value)
            => value 
                ? Result.Success 
                : Result.Fail;
        
        [GarbageFree]
        public static Result<TSuccess> ToResult<TSuccess>(
            this TSuccess value)
            => value;
        
        [GarbageFree]
        public static Result<TSuccess> ToResult<TSuccess>(
            this Failure fail)
            => fail;
        
        [GarbageFree]
        public static Result<TOut> Map<TSuccess, TOut>(
            this Result<TSuccess> result,
            Func<TSuccess, TOut> map)
            => result.IsSuccess
                ? map(result.SuccessValue)
                : result.FailureValue;

        [GarbageFree]
        public static Result<TOut> Map<TSuccess, TOut>(
            this Result<TSuccess> result,
            Func<TSuccess, Result<TOut>> map)
            => result.Match(
                success: map,
                failure: fail => fail);

        [GarbageFree]
        public static Result<TSuccess> OnSuccess<TSuccess>(
            this Result<TSuccess> result,
            Action<TSuccess> action)
        {
            result.Switch(action);
            return result;
        }

        [GarbageFree]
        public static Result OnFailure(
            this Result result,
            Action<Failure> action)
        {
            if (!result.IsSuccess)
                action(result.FailureValue);

            return result;
        }

        [GarbageFree]
        public static Result<TSuccess> OnFailure<TSuccess>(
            this Result<TSuccess> result,
            Action<Failure> action)
        {
            if (!result.IsSuccess)
                action(result.FailureValue);

            return result;
        }

        [GarbageFree]
        public static Result<TSuccess> DefaultWith<TSuccess>(
            this Result<TSuccess> result,
            Func<Failure, TSuccess> fallback)
            => result.IsSuccess
                ? result
                : fallback(result.FailureValue);

        [GarbageFree]
        public static Result<TSuccess> DefaultWith<TSuccess>(
            this Result<TSuccess> result,
            Func<Failure, Result<TSuccess>> fallback)
            => result.Match(
                success: _ => result,
                failure: fallback);
    }
}