using System;
using System.Collections.Generic;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable PossibleMultipleEnumeration

namespace Monads
{
    /// <summary>
    /// These are written to minimize heap allocations compared to linq.
    /// </summary>
    public static class ResultLinqExtensions
    {
        public static void ForEach<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Action<TSuccess> action)
        {
            if (results.IsFailure)
                return;

            foreach (var item in results.SuccessValue)
            {
                action?.Invoke(item);
            }
        }

        public static void ForEach<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Action<TSuccess> action)
        {
            if (results.IsFailure)
                return;

            foreach (var item in results.SuccessValue)
            {
                if (item.IsSuccess)
                    action?.Invoke(item.SuccessValue);
            }
        }

        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, bool> condition)
            => results.IsSuccess
                ? Filter(results.SuccessValue, condition).ToResult()
                : results.FailureValue;

        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, bool> condition)
            => results.IsSuccess
                ? Filter(results.SuccessValue, condition).ToResult()
                : results.FailureValue;

        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, TOut> map)
            => results.IsSuccess
                ? Map(results.SuccessValue, map).ToResult()
                : results.FailureValue;

        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, TOut> map)
            => results.IsSuccess
                ? Map(results.SuccessValue, map).ToResult()
                : results.FailureValue;

        public static Result<IEnumerable<TSuccess>> DistinctBy<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, object> keySelector)
            => results.IsFailure
                ? results
                : KeySingle(results, keySelector);

        private static Result<IEnumerable<TSuccess>> KeySingle<TSuccess>(
            Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, object> keySelector)
        {
            var keys = RentHashSet();

            try
            {
                return MapSingle(results.SuccessValue, keySelector, keys).ToResult();
            }
            finally
            {
                ReturnHashSet(keys); // Ensure the HashSet is returned to the pool
            }
        }

        private static IEnumerable<TSuccess> MapSingle<TSuccess>(
            IEnumerable<TSuccess> results,
            Func<TSuccess, object> keySelector,
            HashSet<object> keys)
        {
            try
            {
                foreach (var item in results)
                {
                    var key = keySelector(item);
                    if (keys.Add(key)) // Check for duplicates using the pooled HashSet
                    {
                        yield return item;
                    }
                }
            }
            finally
            {
                ReturnHashSet(keys); // Ensure the HashSet is returned to the pool
            }
        }

        private static IEnumerable<TSuccess> Filter<TSuccess>(
            IEnumerable<TSuccess> source,
            Func<TSuccess, bool> condition)
        {
            foreach (var success in source)
            {
                if (condition(success))
                    yield return success;
            }
        }

        private static IEnumerable<TSuccess> Filter<TSuccess>(
            this IEnumerable<Result<TSuccess>> source,
            Func<TSuccess, bool> condition)
        {
            foreach (var result in source)
            {
                if (result.IsFailure)
                    continue;

                if (condition(result.SuccessValue))
                    yield return result.SuccessValue;
            }
        }

        private static IEnumerable<TOut> Map<TSuccess, TOut>(
            this IEnumerable<TSuccess> source,
            Func<TSuccess, TOut> map)
        {
            foreach (var success in source)
            {
                yield return map(success);
            }
        }

        private static IEnumerable<TOut> Map<TSuccess, TOut>(
            this IEnumerable<Result<TSuccess>> source,
            Func<TSuccess, TOut> map)
        {
            foreach (var result in source)
            {
                if (result.IsFailure)
                    continue;

                yield return map(result.SuccessValue);
            }
        }

        private static readonly Stack<HashSet<object>> HashSetPool = new();

        private static HashSet<object> RentHashSet()
            => HashSetPool.Count > 0
                ? HashSetPool.Pop()
                : new HashSet<object>();

        private static void ReturnHashSet(HashSet<object> hashSet)
        {
            hashSet.Clear(); // Clear the HashSet before returning it to the pool
            HashSetPool.Push(hashSet);
        }
    }
}