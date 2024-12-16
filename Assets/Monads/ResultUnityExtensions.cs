using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Monads
{
    /// <summary>
    /// Provides extension methods for safely interacting with Unity's GameObject and Component system
    /// using Result monads to ensure null safety and clean error handling.
    /// 
    /// These methods operate on a Result of GameObject (not GameObject directly), treating the GameObject
    /// as an Option-like monad that can either succeed or fail.
    /// 
    /// This approach allows for garbage-free null checks and chaining operations in a fluent style.
    /// </summary>
    public static class ResultUnityExtensions
    {
        /// <summary>
        /// Safely retrieves a component of type TComponent from a Result of GameObject.
        /// 
        /// If the Result represents a failure or the component is not found, a NotFound failure is returned.
        /// </summary>
        /// <typeparam name="TComponent">The type of Component to retrieve.</typeparam>
        /// <param name="source">The Result of GameObject to retrieve the component from.</param>
        /// <returns>
        /// A Result containing the component if found, or a NotFound failure if the component is missing.
        /// </returns>
        [GarbageFree]
        public static Result<TComponent> GetSafeComponent<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.IsSuccess
                ? source.SuccessValue.GetComponent<TComponent>().ToResult() // Converts to Result<TComponent>
                : NotFound.Failure; // Propagate a NotFound failure if the source is already a failure

        /// <summary>
        /// Safely retrieves a component of type TComponent from the children of a Result of GameObject.
        /// 
        /// If the Result represents a failure or the component is not found, a NotFound failure is returned.
        /// </summary>
        /// <typeparam name="TComponent">The type of Component to retrieve.</typeparam>
        /// <param name="source">The Result of GameObject to search in its children.</param>
        /// <returns>
        /// A Result containing the component if found, or a NotFound failure if the component is missing.
        /// </returns>
        [GarbageFree]
        public static Result<TComponent> GetSafeComponentInChildren<TComponent>(
            this Result<GameObject> source) where TComponent : Object
            => source.IsSuccess
                ? source.SuccessValue.GetComponentInChildren<TComponent>().ToResult()
                : NotFound.Failure;

        /// <summary>
        /// Executes an action on a safe component of type TComponent, if it exists.
        /// 
        /// This method combines searching for a component on the GameObject itself and in its children.
        /// If the component is found, the provided action is executed.
        /// </summary>
        /// <typeparam name="TComponent">The type of Component to retrieve.</typeparam>
        /// <param name="source">The Result of GameObject to search for the component.</param>
        /// <param name="action">The action to execute on the retrieved component.</param>
        /// <returns>
        /// The original Result of GameObject. If the component is not found, the result is unchanged.
        /// </returns>
        [GarbageFree]
        public static Result<GameObject> WithSafeComponent<TComponent>(
            this Result<GameObject> source,
            Action<TComponent> action) where TComponent : Object
        {
            if (source.IsFailure)
                return source; // Return early if the source Result is already a failure

            // Try to get the component directly
            var component = source.GetSafeComponent<TComponent>();

            // If the component is not found, look in the children
            if (!component)
                component = source.GetSafeComponentInChildren<TComponent>();

            // If the component exists, execute the provided action
            if (component)
                action(component.SuccessValue);

            return source; // Return the original Result of GameObject for fluent chaining
        }
    }
}
