using System.Collections.Generic;
using Monads;
using UniMediator;
using UnityEngine;

public readonly struct MoveUnit : ISingleMessage<Result<MoveUnitResponse>>
{
    public readonly GameObject Unit;
    public readonly Vector2Int Destination;

    public MoveUnit(GameObject unit, Vector2Int destination)
    {
        Unit = unit;
        Destination = destination;
    }
}

public readonly struct RegisterUnit : ISingleMessage<Result>
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

public readonly struct GetAllUnitPositions : ISingleMessage<Result<GetAllUnitPositionsResponse>> {}

public readonly struct GetAllUnitPositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllUnitPositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}

public readonly struct DetectUnit : ISingleMessage<Result<GameObject>>
{
    public readonly Vector2Int Position;

    public DetectUnit(Vector2Int position)
    {
        Position = position;
    }
}