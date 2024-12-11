using Monads;
using UniMediator;
using UnityEngine;

public readonly struct TargetPosition : ISingleMessage<Result<TargetPositionResponse>>
{
    public readonly Vector2Int Position;

    public TargetPosition(Vector2Int position)
    {
        Position = position;
    }
}

public readonly struct TargetPositionResponse
{
    public readonly GameObject Target;
    public readonly TargetType Type;

    public TargetPositionResponse(
        GameObject target, 
        TargetType type)
    {
        Target = target;
        Type = type;
    }
}

public readonly struct TargetSpotted : IMulticastMessage
{
    public readonly Vector2Int Position;
    public readonly GameObject Target;
    public readonly TargetType Type;

    public TargetSpotted(
        Vector2Int position, 
        GameObject target, 
        TargetType type)
    {
        Position = position;
        Target = target;
        Type = type;
    }
}

public enum TargetType
{
    None = 0,
    Unit,
    Loot,
    Obstacle
}