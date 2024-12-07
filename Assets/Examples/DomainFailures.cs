using Monads;

// easily add your own failure types to your systems
public record LimitExceeded(int Max) : Failure
{
    public const int DEFAULT_MAX = 100;
    public static readonly LimitExceeded Failure = new(DEFAULT_MAX);
}
