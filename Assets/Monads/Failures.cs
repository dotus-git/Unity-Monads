using System;

namespace Monads
{
    // public interface IFailure { }

    public record Failure
    {
        public static readonly Failure Default = new();

        public override string ToString()
            => GetType().Name;
    }

    public record NullReference : Failure
    {
        public static readonly NullReference Failure = new();
    }

    public record NotFound : Failure
    {
        public static readonly NotFound Failure = new();
    }

    public record SystemError : Failure
    {
        public SystemError(Exception exception)
        {
            Message = exception.Message;
        }

        public string Message { get; }

        public override string ToString()
            => Message;
    }
}