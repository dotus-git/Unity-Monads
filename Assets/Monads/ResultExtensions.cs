using System;

namespace Monads
{
    /// <summary>
    /// Provides extension methods for working with <c>Result</c> and <c>Result-TSuccess</c>.
    /// These methods facilitate transformations, mappings, and handling of success and failure cases
    /// while maintaining a garbage-free implementation.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a <c>bool</c> value into a <c>Result</c>.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <returns>A <c>Result</c> representing success if true, or failure if false.</returns>
        [GarbageFree]
        public static Result ToResult(
            this bool value)
            => value 
                ? Result.Success 
                : Result.Fail;

        /// <summary>
        /// Converts a value of type <c>TSuccess</c> into a <c>Result-TSuccess</c>.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <param name="value">The success value to wrap into a result.</param>
        /// <returns>A <c>Result-TSuccess</c> representing success with the provided value.</returns>
        [GarbageFree]
        public static Result<TSuccess> ToResult<TSuccess>(
            this TSuccess value)
            => value;

        /// <summary>
        /// Converts a <c>Failure</c> into a <c>Result-TSuccess</c>.
        /// </summary>
        /// <typeparam name="TSuccess">The success value type.</typeparam>
        /// <param name="fail">The failure value to wrap into a result.</param>
        /// <returns>A <c>Result-TSuccess</c> representing failure with the provided <c>Failure</c> value.</returns>
        [GarbageFree]
        public static Result<TSuccess> ToResult<TSuccess>(
            this Failure fail)
            => fail;

        /// <summary>
        /// Maps a <c>Result-TSuccess</c> into a <c>Result-TOut</c> using a mapping function.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the input success value.</typeparam>
        /// <typeparam name="TOut">The type of the output success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="map">The function to transform the success value.</param>
        /// <returns>
        /// A <c>Result-TOut</c> containing the transformed success value or the original failure.
        /// </returns>
        [GarbageFree]
        public static Result<TOut> Map<TSuccess, TOut>(
            this Result<TSuccess> result,
            Func<TSuccess, TOut> map)
            => result.IsSuccess
                ? map(result.SuccessValue)
                : result.FailureValue;

        /// <summary>
        /// Maps a <c>Result-TSuccess</c> into another <c>Result-TOut</c> using a mapping function
        /// that returns a result.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the input success value.</typeparam>
        /// <typeparam name="TOut">The type of the output success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="map">The function to transform the success value into a result.</param>
        /// <returns>
        /// A <c>Result-TOut</c> returned by the mapping function or the original failure.
        /// </returns>
        [GarbageFree]
        public static Result<TOut> Map<TSuccess, TOut>(
            this Result<TSuccess> result,
            Func<TSuccess, Result<TOut>> map)
            => result.Match(
                success: map,
                failure: fail => fail);

        /// <summary>
        /// Executes an action when the result is successful.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="action">The action to execute on success.</param>
        /// <returns>The original <c>Result-TSuccess</c> for chaining.</returns>
        [GarbageFree]
        public static Result<TSuccess> OnSuccess<TSuccess>(
            this Result<TSuccess> result,
            Action<TSuccess> action)
        {
            result.Switch(action);
            return result;
        }

        /// <summary>
        /// Executes an action when the result is a failure.
        /// </summary>
        /// <param name="result">The input result.</param>
        /// <param name="action">The action to execute on failure.</param>
        /// <returns>The original <c>Result</c> for chaining.</returns>
        [GarbageFree]
        public static Result OnFailure(
            this Result result,
            Action<Failure> action)
        {
            if (!result.IsSuccess)
                action(result.FailureValue);

            return result;
        }

        /// <summary>
        /// Executes an action when the result is a failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="action">The action to execute on failure.</param>
        /// <returns>The original <c>Result-TSuccess</c> for chaining.</returns>
        [GarbageFree]
        public static Result<TSuccess> OnFailure<TSuccess>(
            this Result<TSuccess> result,
            Action<Failure> action)
        {
            if (!result.IsSuccess)
                action(result.FailureValue);

            return result;
        }

        /// <summary>
        /// Returns a default value or result when the current result is a failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="fallback">A function to provide a fallback success value.</param>
        /// <returns>
        /// The original <c>Result-TSuccess</c> if successful, otherwise a fallback result.
        /// </returns>
        [GarbageFree]
        public static Result<TSuccess> DefaultWith<TSuccess>(
            this Result<TSuccess> result,
            Func<Failure, TSuccess> fallback)
            => result.IsSuccess
                ? result
                : fallback(result.FailureValue);

        /// <summary>
        /// Returns a fallback result when the current result is a failure.
        /// </summary>
        /// <typeparam name="TSuccess">The type of the success value.</typeparam>
        /// <param name="result">The input result.</param>
        /// <param name="fallback">A function to provide a fallback result.</param>
        /// <returns>
        /// The original <c>Result-TSuccess</c> if successful, otherwise a fallback result.
        /// </returns>
        [GarbageFree]
        public static Result<TSuccess> DefaultWith<TSuccess>(
            this Result<TSuccess> result,
            Func<Failure, Result<TSuccess>> fallback)
            => result.Match(
                success: _ => result,
                failure: fallback);
    }
}
