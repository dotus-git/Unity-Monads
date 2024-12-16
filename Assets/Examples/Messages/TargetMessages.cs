using UnityEngine;

[MediatorMessage]
public readonly struct TargetPosition 
{
    public readonly Vector2Int Position;

    public TargetPosition(Vector2Int position)
    {
        Position = position;
    }
}

[MediatorMessage]
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

[MediatorMessage]
public readonly struct TargetSpotted 
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