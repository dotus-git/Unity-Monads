using System;
using System.Collections.Generic;

namespace Monads
{
    /// <summary>
    /// Represents a result that encapsulates either success or failure.
    /// This is particularly useful for error handling without using exceptions.
    /// </summary>
    public readonly struct Result :
        IEquatable<Result>,
        IComparable<Result>,
        IComparable
    {
        // Internal storage for the failure value
        private readonly Failure _failureValue;

        /// <summary>
        /// Indicates if the result represents success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates if the result represents failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Represents a successful result.
        /// </summary>
        public static readonly Result Success = new(true);

        /// <summary>
        /// Represents a failed result with a default failure.
        /// </summary>
        public static readonly Result Fail = new(new Failure());

        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <param name="success">Indicates success (true/false).</param>
        public Result(bool success)
        {
            _failureValue = success ? default : Failure.Default;
            IsSuccess = success;
        }

        /// <summary>
        /// Creates a failure result with a given failure value.
        /// </summary>
        /// <param name="failure">The failure value to associate with this result.</param>
        public Result(Failure failure)
        {
            _failureValue = failure;
            IsSuccess = false;
        }

        /// <summary>
        /// Negates the current result, converting success to failure and vice versa.
        /// </summary>
        public Result Negate()
            => new(!IsSuccess);

        /// <summary>
        /// Matches the result, invoking the appropriate function based on success or failure.
        /// Returns a value of type TOut.
        /// </summary>
        /// <typeparam name="TOut">The return type of the match functions.</typeparam>
        /// <param name="success">Function to execute on success.</param>
        /// <param name="failure">Function to execute on failure.</param>
        public TOut Match<TOut>(
            Func<TOut> success,
            Func<Failure, TOut> failure = default)
            => IsSuccess
                ? success == null
                    ? default
                    : success()
                : failure == null
                    ? default
                    : failure(_failureValue);

        /// <summary>
        /// Executes an action based on the result state (success or failure).
        /// </summary>
        /// <param name="success">Action to execute on success.</param>
        /// <param name="failure">Action to execute on failure.</param>
        public void Switch(
            Action success,
            Action<Failure> failure = default)
        {
            if (IsSuccess)
                success?.Invoke();
            else
                failure?.Invoke(_failureValue);
        }

        /// <summary>
        /// Gets the failure value if the result represents failure.
        /// Throws an exception if accessed on success.
        /// </summary>
        public Failure FailureValue
        {
            get
            {
                if (!IsFailure)
                    throw new InvalidOperationException("Cannot access FailureValue when Result is a success.");

                return _failureValue;
            }
        }

        /// <summary>
        /// Implicitly converts a boolean to a Result, evaluating to success if true. 
        /// </summary>
        public static implicit operator Result(bool result)
            => new(result);

        /// <summary>
        /// Implicitly converts a Result to a boolean, evaluating to true if success.
        /// </summary>
        public static implicit operator bool(Result result)
            => result.IsSuccess;

        /// <summary>
        /// Implicitly converts a Failure into a failed Result.
        /// </summary>
        public static implicit operator Result(Failure failure)
            => new(failure);

        /// <summary>
        /// Implicitly converts a Result into a Failure, throwing an exception if the result is a success.
        /// </summary>
        public static implicit operator Failure(Result result)
            => result.IsFailure
                ? result.FailureValue
                : throw new InvalidOperationException("Cannot convert a successful result to a Failure.");

        /// <summary>
        /// Provides a string representation of the result ("success" or the failure value).
        /// </summary>
        public override string ToString()
            => IsSuccess
                ? "success"
                : _failureValue?.ToString() ?? "";

        /// <summary>
        /// Determines if this Result is equal to another Result.
        /// </summary>
        public bool Equals(Result other)
            => IsSuccess == other.IsSuccess &&
               Equals(_failureValue, other._failureValue);

        /// <inheritdoc />
        public override bool Equals(object obj)
            => obj is Result other && Equals(other);

        /// <summary>
        /// Compares two results.
        /// Success is considered "greater" than failure.
        /// If both are failures, the failure values are compared.
        /// </summary>
        public int CompareTo(Result other)
        {
            if (IsSuccess && !other.IsSuccess) return 1;
            if (!IsSuccess && other.IsSuccess) return -1;
            if (IsSuccess && other.IsSuccess) return 0;

            return Comparer<object>.Default.Compare(_failureValue, other._failureValue);
        }

        /// <summary>
        /// Implementation for IComparable, allowing comparisons with non-typed objects.
        /// </summary>
        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is not Result other)
                throw new ArgumentException("Object is not of type Result");

            return CompareTo(other);
        }

        /// <summary>
        /// Computes a hash code for this Result.
        /// Success has a fixed hash code of 1, while failure uses the failure value's hash.
        /// </summary>
        public override int GetHashCode()
            => IsSuccess
                ? 1
                : _failureValue.GetHashCode();

        /// <summary>
        /// Equality operator for comparing two Results.
        /// </summary>
        public static bool operator ==(Result left, Result right)
            => left.Equals(right);

        /// <summary>
        /// Inequality operator for comparing two Results.
        /// </summary>
        public static bool operator !=(Result left, Result right)
            => !left.Equals(right);
    }
}
