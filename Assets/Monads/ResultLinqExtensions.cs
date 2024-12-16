using System;
using System.Collections.Generic;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable PossibleMultipleEnumeration

namespace Monads
{
    /// <summary>
    /// Provides LINQ-like extensions for Result and IEnumerable-Result while minimizing heap allocations.
    /// 
    /// These methods avoid traditional LINQ overhead by using explicit loops and pooled objects
    /// to reduce garbage collection in Unity or performance-critical environments.
    /// </summary>
    public static class ResultLinqExtensions
    {
        /// <summary>
        /// Performs an action on each item in a Result of IEnumerable.
        /// </summary>
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

        /// <summary>
        /// Performs an action on each successful item in a Result of IEnumerable of Result.
        /// </summary>
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

        /// <summary>
        /// Filters the successful items of a Result of IEnumerable using the specified condition.
        /// </summary>
        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, bool> condition)
            => results.IsSuccess
                ? Filter(results.SuccessValue, condition).ToResult()
                : results.FailureValue;

        /// <summary>
        /// Filters successful results in an IEnumerable of Result using the specified condition.
        /// </summary>
        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, bool> condition)
            => results.IsSuccess
                ? Filter(results.SuccessValue, condition).ToResult()
                : results.FailureValue;

        /// <summary>
        /// Transforms a Result of IEnumerable into a Result of IEnumerable of mapped values.
        /// </summary>
        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, TOut> map)
            => results.IsSuccess
                ? Map(results.SuccessValue, map).ToResult()
                : results.FailureValue;

        /// <summary>
        /// Transforms successful items in a Result of IEnumerable of Result using the specified mapping function.
        /// </summary>
        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, TOut> map)
            => results.IsSuccess
                ? Map(results.SuccessValue, map).ToResult()
                : results.FailureValue;

        /// <summary>
        /// Returns distinct items in a Result of IEnumerable based on a key selector function.
        /// Uses a pooled HashSet to minimize garbage.
        /// </summary>
        public static Result<IEnumerable<TSuccess>> DistinctBy<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, object> keySelector)
            => results.IsFailure
                ? results
                : KeySingle(results, keySelector);

        /// <summary>
        /// Internal helper method for distinct filtering using a HashSet.
        /// </summary>
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
                ReturnHashSet(keys); // Return the HashSet to the pool
            }
        }

        /// <summary>
        /// Filters distinct values from the source using a pooled HashSet to minimize garbage.
        /// </summary>
        private static IEnumerable<TSuccess> MapSingle<TSuccess>(
            IEnumerable<TSuccess> results,
            Func<TSuccess, object> keySelector,
            HashSet<object> keys)
        {
            foreach (var item in results)
            {
                var key = keySelector(item);
                if (keys.Add(key)) // Add to HashSet only if the key is unique
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Filters values based on a condition.
        /// </summary>
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

        /// <summary>
        /// Filters values from IEnumerable of Result based on a condition.
        /// </summary>
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

        /// <summary>
        /// Transforms values using a mapping function.
        /// </summary>
        private static IEnumerable<TOut> Map<TSuccess, TOut>(
            this IEnumerable<TSuccess> source,
            Func<TSuccess, TOut> map)
        {
            foreach (var success in source)
            {
                yield return map(success);
            }
        }

        /// <summary>
        /// Transforms successful values in an IEnumerable of Result using a mapping function.
        /// </summary>
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

        /// <summary>
        /// A pool of HashSet objects to minimize garbage collection.
        /// </summary>
        private static readonly Stack<HashSet<object>> HashSetPool = new();

        /// <summary>
        /// Rents a HashSet from the pool or creates a new one if the pool is empty.
        /// </summary>
        private static HashSet<object> RentHashSet()
            => HashSetPool.Count > 0
                ? HashSetPool.Pop()
                : new HashSet<object>();

        /// <summary>
        /// Returns a HashSet to the pool after clearing it.
        /// </summary>
        private static void ReturnHashSet(HashSet<object> hashSet)
        {
            hashSet.Clear(); // Clear the HashSet before returning it to the pool
            HashSetPool.Push(hashSet);
        }
    }
}
