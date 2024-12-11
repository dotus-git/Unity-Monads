using System.Collections.Generic;
using System.Linq;
using Monads;
using UniMediator;
using UnityEngine;
using UnityUtils;

public class ObstacleSystem : Singleton<ObstacleSystem>,
    ISingleMessageHandler<RegisterObstacle, Result>,
    ISingleMessageHandler<DetectObstacle, Result<GameObject>>,
    ISingleMessageHandler<GetAllObstaclePositions, Result<GetAllObstaclePositionsResponse>>
{
    private readonly Dictionary<Vector2Int, GameObject> _obstacles = new();
    public int Count;

    public Result Handle(RegisterObstacle message)
    {
        var gridPosition = message.Obstacle.transform.position.ToV2I();
        if (!_obstacles.TryAdd(gridPosition, message.Obstacle))
        {
            // in this scenario, we don't need to pass back any data, 
            // the obstacle trying to register itself already has its own info.
            return Result.Fail;
        }

        Count = _obstacles.Count;
        
        return Result.Success;
    }

    public Result<GameObject> Handle(DetectObstacle message)
        => _obstacles.TryGetValue(message.Position, out var obstacle)
            ? obstacle.ToResult()
            : Failure.Default;

    public Result<GetAllObstaclePositionsResponse> Handle(GetAllObstaclePositions message)
        => new GetAllObstaclePositionsResponse(_obstacles.Keys.AsEnumerable());
}