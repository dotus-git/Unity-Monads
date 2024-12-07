using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace Monads
{
    /// <summary>
    /// These methods are not garbage-free.
    /// They create new collections and hash sets.
    /// They are written to minimize heap allocations compared to linq.
    /// </summary>
    public static class ResultCollectionExtensions
    {
        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, bool> condition)
            => results.Match(
                success =>
                {
                    var outputList = new List<TSuccess>(success.Count());
                    foreach (var item in success)
                    {
                        if (condition(item))
                            outputList.Add(item);
                    }

                    return new Result<IEnumerable<TSuccess>>(outputList);
                },
                failure => new Result<IEnumerable<TSuccess>>(failure)
            );

        public static Result<IEnumerable<TSuccess>> Where<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, bool> condition)
            => results.Match(
                success =>
                {
                    var outputList = new List<TSuccess>(success.Count());
                    foreach (var item in success)
                    {
                        item.Switch(
                            successItem =>
                            {
                                if (condition(successItem))
                                    outputList.Add(successItem);
                            }
                        );
                    }

                    return new Result<IEnumerable<TSuccess>>(outputList);
                },
                failure => new Result<IEnumerable<TSuccess>>(failure)
            );

        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, TOut> map)
            => results.Match(
                success =>
                {
                    var outputList = new List<TOut>(success.Count());
                    foreach (var item in success)
                    {
                        outputList.Add(map(item));
                    }

                    return new Result<IEnumerable<TOut>>(outputList);
                },
                failure => new Result<IEnumerable<TOut>>(failure)
            );

        public static Result<IEnumerable<TOut>> Select<TSuccess, TOut>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<TSuccess, TOut> map)
            => results.Match(
                success =>
                {
                    var outputList = new List<TOut>(success.Count());
                    foreach (var item in success)
                    {
                        item.Switch(
                            successItem => outputList.Add(map(successItem))
                        );
                    }

                    return new Result<IEnumerable<TOut>>(outputList);
                },
                failure => new Result<IEnumerable<TOut>>(failure)
            );

        public static Result<IEnumerable<TSuccess>> ClearFailures<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results)
            => results.Match(
                success =>
                {
                    var outputList = new List<TSuccess>(success.Count());
                    foreach (var item in success)
                    {
                        item.Switch(
                            successItem => { outputList.Add(successItem); }
                        );
                    }

                    return new Result<IEnumerable<TSuccess>>(outputList);
                },
                failure => new Result<IEnumerable<TSuccess>>(failure)
            );

        public static void ForEach<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Action<TSuccess> action)
            => results.Switch(
                success =>
                {
                    foreach (var item in success)
                    {
                        item.Switch(
                            action
                        );
                    }
                }
            );

        public static Result<IEnumerable<TSuccess>> DefaultEachWith<TSuccess>(
            this Result<IEnumerable<Result<TSuccess>>> results,
            Func<Failure, TSuccess> fallback)
            => results.Match(
                success =>
                {
                    var outputList = new List<TSuccess>(success.Count());
                    foreach (var item in success)
                    {
                        item.Switch(
                            successItem => outputList.Add(successItem),
                            failure => outputList.Add(fallback(failure))
                        );
                    }

                    return new Result<IEnumerable<TSuccess>>(outputList);
                },
                failure => new Result<IEnumerable<TSuccess>>(failure)
            );
        
        public static Result<IEnumerable<TSuccess>> DistinctBy<TSuccess> (
            this Result<IEnumerable<TSuccess>> results,
            Func<TSuccess, object> keySelector)
            => results.Match(
                success =>
                {
                    var outputList = new List<TSuccess>(success.Count());
                    var keys = new HashSet<object>(); // creates an extra garbage-collected HashSet
                    foreach (var item in success)
                    {
                        var key = keySelector(item);
                        if (keys.Add(key))
                            outputList.Add(item);
                    }

                    return new Result<IEnumerable<TSuccess>>(outputList);
                },
                failure => new Result<IEnumerable<TSuccess>>(failure)
            );
    }
}