using System.Collections.Generic;
using UnityEngine;

[MediatorMessage]
public struct RegisterObstacle
{
    public readonly GameObject Obstacle;

    public RegisterObstacle(GameObject obstacle)
    {
        Obstacle = obstacle;
    }
}

[MediatorMessage]
public struct DetectObstacle
{
    public readonly Vector2Int Position;

    public DetectObstacle(Vector2Int position)
    {
        Position = position;
    }
}

[MediatorMessage]
public struct GetAllObstaclePositions {}

public struct GetAllObstaclePositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllObstaclePositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}