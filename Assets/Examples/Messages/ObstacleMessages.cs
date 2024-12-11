using System.Collections.Generic;
using Monads;
using UniMediator;
using UnityEngine;

public struct RegisterObstacle : ISingleMessage<Result>
{
    public readonly GameObject Obstacle;

    public RegisterObstacle(GameObject obstacle)
    {
        Obstacle = obstacle;
    }
}

public struct DetectObstacle : ISingleMessage<Result<GameObject>>
{
    public readonly Vector2Int Position;

    public DetectObstacle(Vector2Int position)
    {
        Position = position;
    }
}

public struct GetAllObstaclePositions : ISingleMessage<Result<GetAllObstaclePositionsResponse>> {}

public struct GetAllObstaclePositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllObstaclePositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}