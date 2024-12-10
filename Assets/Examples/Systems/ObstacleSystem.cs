using System.Collections.Generic;
using Dotus.Core;
using Monads;
using UniMediator;
using UnityEngine;
using UnityUtils;

public class ObstacleSystem : Singleton<ObstacleSystem>,
    ISingleMessageHandler<RegisterObstacle, Result>,
    ISingleMessageHandler<DetectObstacle, Result<GameObject>>
{
    private readonly Dictionary<Vector2Int, GameObject> Obstacles = new();
    public int Count;

    private void Start()
    {
        Instance.Obstacles.Clear();
    }

    public Result Handle(RegisterObstacle message)
    {
        var gridPosition = message.Obstacle.transform.position.ToV2I();
        if (!Instance.Obstacles.TryAdd(gridPosition, message.Obstacle.gameObject))
        {
            // in this scenario, we don't need to pass back any data, 
            // the obstacle trying to register itself already has its own info.
            return Result.Fail;
        }

        Instance.Count = Obstacles.Count;
        
        return Result.Success;
    }

    public Result<GameObject> Handle(DetectObstacle message)
        => Instance.Obstacles.TryGetValue(message.Position, out var obstacle)
            ? obstacle.ToResult()
            : Failure.Default;
}