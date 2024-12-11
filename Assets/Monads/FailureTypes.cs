using System;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedPositionalProperty.Global

namespace Monads
{
    // public interface IFailure { }

    public record Failure
    {
        // if you only use the static readonly fields, you won't generate garbage
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

    public record ValidationFailure(string Field, string Reason) : Failure;

    public record Unauthorized : Failure
    {
        public static readonly Unauthorized Failure = new();
    }

    public record Forbidden : Failure
    {
        public static readonly Forbidden Failure = new();
    }

    public record Timeout : Failure
    {
        public static readonly Timeout Failure = new();
    }

    public record Conflict(string ConflictReason) : Failure;

    public record InvalidState(string StateDescription) : Failure;

    public record NotImplemented : Failure
    {
        public static readonly NotImplemented Failure = new();
    }

    public record ResourceExhausted(string ResourceName) : Failure;

    public record DependencyFailure(string DependencyName, string Message) : Failure;

    public record InvalidInput(string InputName, string Reason) : Failure;

    public record AlreadyExists(string Item) : Failure;

    public record Cancelled : Failure
    {
        public static readonly Cancelled Failure = new();
    }

    public record ParseFailure(string Input, string TargetType) : Failure;

    public record NetworkFailure(string Reason) : Failure;

}