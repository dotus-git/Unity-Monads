using System.Collections.Generic;
using UnityEngine;

[MediatorMessage]
public readonly struct MoveUnit
{
    public readonly GameObject Unit;
    public readonly Vector2Int Destination;

    public MoveUnit(GameObject unit, Vector2Int destination)
    {
        Unit = unit;
        Destination = destination;
    }
}

[MediatorMessage]
public readonly struct RegisterUnit
{
    public readonly GameObject Unit;

    public RegisterUnit(GameObject unit)
    {
        Unit = unit;
    }
}

public readonly struct MoveUnitResponse
{
    public readonly Vector2Int LandingPosition;
    public readonly int ActionPoints;

    public MoveUnitResponse(Vector2Int landingPosition, int actionPoints)
    {
        LandingPosition = landingPosition;
        ActionPoints = actionPoints;
    }
}

[MediatorMessage]
public readonly struct GetAllUnitPositions {}

[MediatorMessage]
public readonly struct GetAllUnitPositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllUnitPositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}

[MediatorMessage]
public readonly struct DetectUnit
{
    public readonly Vector2Int Position;

    public DetectUnit(Vector2Int position)
    {
        Position = position;
    }
}