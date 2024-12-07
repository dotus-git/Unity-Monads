using System;
using UnityEngine;
using Object = UnityEngine.Object;
// ReSharper disable UnusedType.Global

namespace Monads
{
    public static class ResultUnityExtensions
    {
        [GarbageFree]
        public static Result<GameObject> ToResult(
            this GameObject value)
            => value
                ? new Result<GameObject>(value)
                : NotFound.Failure;

        [GarbageFree]
        public static Result<Transform> ToResult(
            this Transform value)
            => value
                ? new Result<Transform>(value)
                : NotFound.Failure;

        // Because we extend Result<GameObject> source and not GameObject source,
        // source itself is semantically an Option<T> at this point,
        // so we can use source.Match to handle nullability
        [GarbageFree]
        public static Result<TComponent> GetSafeComponent<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.Match(
                success => success.GetComponent<TComponent>().ToResult(),
                _ => NotFound.Failure);

        [GarbageFree]
        public static Result<TComponent> GetSafeComponentInChildren<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.Match(
                success => success.GetComponentInChildren<TComponent>().ToResult(),
                _ => NotFound.Failure);

        [GarbageFree]
        public static void WithSafeComponent<TComponent>(
            this Result<GameObject> source,
            Action<TComponent> action) where TComponent : Object
            => source
                .GetSafeComponent<TComponent>()
                .DefaultWith(_ => source.GetSafeComponentInChildren<TComponent>())
                .Do(action)
                .DoWhenFailure(fail => Debug.LogWarning($"Expected component {typeof(TComponent)} on {source} but got {fail}"));

        [GarbageFree]
        public static void WithSafeComponent<TComponent>(
            this GameObject source,
            Action<TComponent> action) where TComponent : Object
            => source
                .ToResult()
                .WithSafeComponent(action);
    }
}
            