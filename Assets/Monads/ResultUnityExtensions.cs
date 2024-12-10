using System;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedType.Global

namespace Monads
{
    public static class ResultUnityExtensions
    {
        // Because we extend Result<GameObject> source and not GameObject source,
        // source itself is semantically an Option<T> at this point,
        // so we can use source.Match to handle nullability
        [GarbageFree]
        public static Result<TComponent> GetSafeComponent<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.IsSuccess
                ? source.SuccessValue.GetComponent<TComponent>().ToResult()
                : NotFound.Failure;

        [GarbageFree]
        public static Result<TComponent> GetSafeComponentInChildren<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.IsSuccess
                ? source.SuccessValue.GetComponentInChildren<TComponent>().ToResult()
                : NotFound.Failure;

        [GarbageFree]
        public static Result<GameObject> WithSafeComponent<TComponent>(
            this Result<GameObject> source,
            Action<TComponent> action) where TComponent : Object
        {
            if (source.IsFailure)
                return source;

            var component = source.GetSafeComponent<TComponent>();
            if (!component)
                component = source.GetSafeComponentInChildren<TComponent>();

            if (component)
                action(component.SuccessValue);

            return source;
        }
    }
}