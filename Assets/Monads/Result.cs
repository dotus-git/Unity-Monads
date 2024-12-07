using System;
using System.Collections.Generic;

namespace Monads
{
    // This Result struct is a simple implementation of the Railway Oriented Programming pattern.
    // We are focused on railway and being garbage-free in Unity.
    public readonly struct Result<TSuccess> :
        IEquatable<Result<TSuccess>>,
        IEquatable<TSuccess>,
        IComparable<Result<TSuccess>>,
        IComparable<TSuccess>,
        IComparable
    {
        private readonly TSuccess _successValue;
        private readonly Failure _failureValue;
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public Result(Failure failure)
        {
            _failureValue = failure;
            _successValue = default;
            IsSuccess = false;
        }

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

        [GarbageFree]
        public TOut Match<TOut>(
            Func<TSuccess, TOut> success,
            Func<Failure, TOut> failure = default)
            => IsSuccess
                ? success == null
                    ? default
                    : success(_successValue)
                : failure == null
                    ? default
                    : failure(_failureValue);

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

        public TSuccess SuccessValue
        {
            get
            {
                if (!IsSuccess)
                    throw new InvalidOperationException("Cannot access SuccessValue when Result is a failure.");

                return _successValue;
            }
        }

        public Failure FailureValue
        {
            get
            {
                if (!IsFailure)
                    throw new InvalidOperationException("Cannot access FailureValue when Result is a success.");

                return _failureValue;
            }
        }

        // implicit operator to bool where true means success
        public static implicit operator bool(Result<TSuccess> result)
            => result.IsSuccess;

        // implicit operators to allow implicit conversion between Result<Failure, TSuccess> and TSuccess
        public static implicit operator Result<TSuccess>(TSuccess success)
            => new(success);

        public static implicit operator Result<TSuccess>(Failure failure)
            => new(failure);

        public static implicit operator Failure(Result<TSuccess> result)
            => result.IsFailure ? result.FailureValue : throw new InvalidOperationException("Cannot convert a successful result to a Failure.");

        public override string ToString()
            => IsSuccess
                ? _successValue?.ToString() ?? ""
                : _failureValue?.ToString() ?? "";

        public bool Equals(Result<TSuccess> other)
            => EqualityComparer<TSuccess>.Default.Equals(_successValue, other._successValue) && Equals(_failureValue, other._failureValue) && IsSuccess == other.IsSuccess;

        public bool Equals(TSuccess other)
            => IsSuccess && EqualityComparer<TSuccess>.Default.Equals(_successValue, other);

        public override bool Equals(object obj)
            => obj is Result<TSuccess> other && Equals(other);

        public int CompareTo(Result<TSuccess> other)
        {
            if (IsSuccess && !other.IsSuccess) return 1;
            if (!IsSuccess && other.IsSuccess) return -1;
            if (!IsSuccess && !other.IsSuccess) return 0; // Assume all failures are equivalent

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

        public override int GetHashCode()
            => IsSuccess
                ? _successValue.GetHashCode()
                : _failureValue.GetHashCode();

        public static bool operator ==(Result<TSuccess> left, Result<TSuccess> right)
            => left.Equals(right);

        public static bool operator !=(Result<TSuccess> left, Result<TSuccess> right)
            => !left.Equals(right);
    }
}