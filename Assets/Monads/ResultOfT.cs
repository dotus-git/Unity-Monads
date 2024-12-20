using System;
using System.Collections.Generic;

namespace Monads
{
    /// <summary>
    /// Represents a Result monad with a success value of type TSuccess or a Failure value.
    /// 
    /// This implementation follows the principles of Railway-Oriented Programming (ROP):
    /// - Success and Failure are mutually exclusive.
    /// - It avoids exceptions for control flow and explicitly models failure cases.
    /// 
    /// Designed with Unity in mind:
    /// - Avoids unnecessary garbage collection (garbage-free).
    /// - Integrates safely with Unity's null checks (Unity's "fake null" issues are addressed).
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    public readonly struct Result<TSuccess> :
        IEquatable<Result<TSuccess>>,
        IEquatable<TSuccess>,
        IComparable<Result<TSuccess>>,
        IComparable<TSuccess>,
        IComparable
    {
        // The success value, stored when IsSuccess is true
        private readonly TSuccess _successValue;

        // The failure value, stored when IsFailure is true
        private readonly Failure _failureValue;

        /// <summary>
        /// Indicates if this Result represents success.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicates if this Result represents failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Constructs a Result representing a failure.
        /// </summary>
        public Result(Failure failure)
        {
            _failureValue = failure;
            _successValue = default;
            IsSuccess = false;
        }

        /// <summary>
        /// Constructs a Result representing a success.
        /// </summary>
        /// <remarks>
        /// A null success value is treated as a failure (opinionated).
        /// This avoids accidental null propagation and aligns with the <c>Option-T</c> pattern.
        /// 
        /// Unity "fake nulls" are handled to prevent issues with destroyed objects.
        /// </remarks>
        public Result(TSuccess success)
        {
            // This is opinionated code.
            // This code assumes that a null success value is always a NullReference failure condition.
            // As a result, we get Option<T> Some/None for free inside our Result<TSuccess>
            //
            // Object.Equals(null) will sidestep Unity’s “fake null” implementation
            // Unity Objects that are being destroyed should evaluate to failure
            // https://docs.unity3d.com/6000.0/Documentation/Manual/null-reference-exception.html
            if (Equals(success, null) || success.Equals(null))
            {
                _successValue = default;
                _failureValue = NullReference.Failure;
                IsSuccess = false;
                return;
            }

            _successValue = success;
            _failureValue = default;
            IsSuccess = true;
        }

        /// <summary>
        /// Matches the Result to produce a value, depending on success or failure.
        /// </summary>
        /// <typeparam name="TOut">The return type of the match functions.</typeparam>
        /// <param name="success">Function invoked on success.</param>
        /// <param name="failure">Function invoked on failure (optional).</param>
        /// <returns>The result of the invoked function.</returns>
        [GarbageFree]
        public TOut Match<TOut>(
            Func<TSuccess, TOut> success,
            Func<Failure, TOut> failure = default)
            => IsSuccess
                ? success == null ? default : success(_successValue)
                : failure == null ? default : failure(_failureValue);

        /// <summary>
        /// Executes an action depending on success or failure.
        /// </summary>
        /// <param name="success">Action to execute on success.</param>
        /// <param name="failure">Action to execute on failure (optional).</param>
        [GarbageFree]
        public void Switch(
            Action<TSuccess> success,
            Action<Failure> failure = default)
        {
            if (IsSuccess)
                success?.Invoke(_successValue);
            else
                failure?.Invoke(_failureValue);
        }

        /// <summary>
        /// Gets the success value. Throws an exception if accessed on failure.
        /// </summary>
        public TSuccess SuccessValue
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot access SuccessValue when Result is a failure.");
                return _successValue;
            }
        }

        /// <summary>
        /// Gets the failure value. Throws an exception if accessed on success.
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
        /// Implicitly converts the Result to a boolean, where true indicates success.
        /// </summary>
        public static implicit operator bool(Result<TSuccess> result)
            => result.IsSuccess;

        /// <summary>
        /// Implicitly converts a success value to a Result.
        /// </summary>
        public static implicit operator Result<TSuccess>(TSuccess success)
            => new(success);

        /// <summary>
        /// Implicitly converts a Failure to a Result.
        /// </summary>
        public static implicit operator Result<TSuccess>(Failure failure)
            => new(failure);

        /// <summary>
        /// Implicitly converts a failed Result to a Failure.
        /// Throws an exception if the Result is successful.
        /// </summary>
        public static implicit operator Failure(Result<TSuccess> result)
            => result.IsFailure
                ? result.FailureValue
                : throw new InvalidOperationException("Cannot convert a successful result to a Failure.");

        /// <summary>
        /// Returns a string representation of the Result.
        /// </summary>
        public override string ToString()
            => IsSuccess
                ? _successValue?.ToString() ?? ""
                : _failureValue?.ToString() ?? "";

        /// <summary>
        /// Compares two Results for equality.
        /// </summary>
        public bool Equals(Result<TSuccess> other)
            => EqualityComparer<TSuccess>.Default.Equals(_successValue, other._successValue)
               && Equals(_failureValue, other._failureValue)
               && IsSuccess == other.IsSuccess;

        /// <summary>
        /// Compares the success value for equality.
        /// </summary>
        public bool Equals(TSuccess other)
            => IsSuccess && EqualityComparer<TSuccess>.Default.Equals(_successValue, other);

        public override bool Equals(object obj)
            => obj is Result<TSuccess> other && Equals(other);

        public override int GetHashCode()
            => IsSuccess
                ? _successValue.GetHashCode()
                : _failureValue.GetHashCode();

        public static bool operator ==(Result<TSuccess> left, Result<TSuccess> right)
            => left.Equals(right);

        public static bool operator !=(Result<TSuccess> left, Result<TSuccess> right)
            => !left.Equals(right);

        /// <summary>
        /// Compares two Results, treating success as greater than failure.
        /// </summary>
        public int CompareTo(Result<TSuccess> other)
        {
            if (IsSuccess && !other.IsSuccess) return 1;
            if (!IsSuccess && other.IsSuccess) return -1;
            if (!IsSuccess && !other.IsSuccess) return 0;

            return Comparer<TSuccess>.Default.Compare(_successValue, other._successValue);
        }

        public int CompareTo(TSuccess other)
            => IsSuccess
                ? Comparer<TSuccess>.Default.Compare(_successValue, other)
                : -1;

        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is not Result<TSuccess> other)
                throw new ArgumentException("Object is not of type Result<TSuccess>");
            return CompareTo(other);
        }
    }
}
