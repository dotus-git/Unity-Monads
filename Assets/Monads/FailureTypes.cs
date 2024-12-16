using System;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable NotAccessedPositionalProperty.Global

namespace Monads
{
    /// <summary>
    /// Represents the base class for all failure states in the Result monad.
    /// This class and its derived types are designed to be garbage-free and efficient.
    /// 
    /// Key points:
    /// - All derived types are records, which provide built-in equality and immutability.
    /// - Static readonly instances are used to avoid heap allocations for common failure types.
    /// - Custom failures can include additional information (e.g., error messages or field names).
    /// </summary>
    public record Failure
    {
        /// <summary>
        /// A default, generic failure instance.
        /// Use this for cases where no specific failure details are required.
        /// </summary>
        public static readonly Failure Default = new();

        /// <summary>
        /// Returns the name of the failure type as its string representation.
        /// </summary>
        public override string ToString()
            => GetType().Name;
    }

    /// <summary>
    /// Represents a null reference failure.
    /// This is useful when a success value is unexpectedly null.
    /// </summary>
    public record NullReference : Failure
    {
        public static readonly NullReference Failure = new();
    }

    /// <summary>
    /// Represents a "not found" failure.
    /// This can be used when a requested resource or item cannot be found.
    /// </summary>
    public record NotFound : Failure
    {
        public static readonly NotFound Failure = new();
    }

    /// <summary>
    /// Represents a system error caused by an exception.
    /// Stores the exception message for debugging purposes.
    /// </summary>
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

    /// <summary>
    /// Represents a validation failure with a specific field and reason.
    /// This is useful for form validation or input errors.
    /// </summary>
    public record ValidationFailure(string Field, string Reason) : Failure;

    /// <summary>
    /// Represents an unauthorized access failure.
    /// </summary>
    public record Unauthorized : Failure
    {
        public static readonly Unauthorized Failure = new();
    }

    /// <summary>
    /// Represents a forbidden operation failure.
    /// This indicates that the user does not have permission to perform the action.
    /// </summary>
    public record Forbidden : Failure
    {
        public static readonly Forbidden Failure = new();
    }

    /// <summary>
    /// Represents a timeout failure.
    /// Use this when an operation exceeds the allowed time.
    /// </summary>
    public record Timeout : Failure
    {
        public static readonly Timeout Failure = new();
    }

    /// <summary>
    /// Represents a conflict failure with a specific reason.
    /// </summary>
    public record Conflict(string ConflictReason) : Failure;

    /// <summary>
    /// Represents an invalid state failure with a description.
    /// </summary>
    public record InvalidState(string StateDescription) : Failure;

    /// <summary>
    /// Represents a "not implemented" failure.
    /// Use this as a placeholder for incomplete functionality.
    /// </summary>
    public record NotImplemented : Failure
    {
        public static readonly NotImplemented Failure = new();
    }

    /// <summary>
    /// Represents a failure due to resource exhaustion.
    /// Includes the name of the resource that was exhausted.
    /// </summary>
    public record ResourceExhausted(string ResourceName) : Failure;

    /// <summary>
    /// Represents a failure caused by a dependency, including its name and an error message.
    /// </summary>
    public record DependencyFailure(string DependencyName, string Message) : Failure;

    /// <summary>
    /// Represents an invalid input failure with a specific input name and reason.
    /// </summary>
    public record InvalidInput(string InputName, string Reason) : Failure;

    /// <summary>
    /// Represents a failure due to a resource or item that already exists.
    /// </summary>
    public record AlreadyExists(string Item) : Failure;

    /// <summary>
    /// Represents a cancellation failure.
    /// Use this when an operation is intentionally canceled.
    /// </summary>
    public record Cancelled : Failure
    {
        public static readonly Cancelled Failure = new();
    }

    /// <summary>
    /// Represents a failure during parsing, including the input value and the target type.
    /// </summary>
    public record ParseFailure(string Input, string TargetType) : Failure;

    /// <summary>
    /// Represents a network-related failure with a specific reason.
    /// </summary>
    public record NetworkFailure(string Reason) : Failure;
}
