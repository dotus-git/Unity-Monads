using System;
using System.Collections.Generic;

namespace Monads
{
    public readonly struct Result :
        IEquatable<Result>,
        IComparable<Result>,
        IComparable
    {
        private readonly Failure _failureValue;
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public static readonly Result Success = new(true);
        public static readonly Result Fail = new(new Failure());

        public Result(bool success)
        {
            _failureValue = success ? default : Failure.Default;
            IsSuccess = success;
        }
        
        public Result(Failure failure)
        {
            _failureValue = failure;
            IsSuccess = false;
        }
        
        public Result Negate()
            => new(!IsSuccess);

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

        public void Switch(
            Action success,
            Action<Failure> failure = default)
        {
            if (IsSuccess)
                success?.Invoke();
            else
                failure?.Invoke(_failureValue);
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

        public static implicit operator bool(Result result)
            => result.IsSuccess;

        public static implicit operator Result(Failure failure)
            => new(failure);

        public static implicit operator Failure(Result result)
            => result.IsFailure
                ? result.FailureValue
                : throw new InvalidOperationException("Cannot convert a successful result to a Failure.");

        public override string ToString()
            => IsSuccess
                ? "success"
                : _failureValue?.ToString() ?? "";

        public bool Equals(Result other)
            => IsSuccess == other.IsSuccess && 
               Equals(_failureValue, other._failureValue);

        public override bool Equals(object obj)
            => obj is Result other && Equals(other);

        public int CompareTo(Result other)
        {
            if (IsSuccess && !other.IsSuccess) return 1;
            if (!IsSuccess && other.IsSuccess) return -1;
            if (IsSuccess && other.IsSuccess) return 0;

            return Comparer<object>.Default.Compare(_failureValue, other._failureValue);
        }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null) return 1;

            if (obj is not Result other)
                throw new ArgumentException("Object is not of type Result");

            return CompareTo(other);
        }

        public override int GetHashCode()
            => IsSuccess
                ? 1
                : _failureValue.GetHashCode();

        public static bool operator ==(Result left, Result right)
            => left.Equals(right);

        public static bool operator !=(Result left, Result right)
            => !left.Equals(right);
    }
}