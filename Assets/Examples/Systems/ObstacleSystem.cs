using System.Collections.Generic;
using System.Linq;
using Monads;
using UnityEngine;
using UnityUtils;

// note: this approach is more 'traditional' in code approach,
// while the LootSystem showcases the fluent approach
// The LootSystem shows how most Handlers boil down to a single line of code
// This makes digesting the Handlers easier, especially in larger projects
public class ObstacleSystem : Singleton<ObstacleSystem>
{
    private readonly Dictionary<Vector2Int, GameObject> _obstacles = new Dictionary<Vector2Int, GameObject>();
    public int Count;

    protected override void Awake()
    {
        base.Awake();
        _obstacles.Clear();
    }

    [MediatorHandler]
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

    [MediatorHandler]
    public Result<GameObject> Handle(DetectObstacle message)
    {
        if (_obstacles.TryGetValue(message.Position, out var obstacle))
            return obstacle.ToResult();
        else
            return Failure.Default;
    }

    [MediatorHandler]
    public Result<GetAllObstaclePositionsResponse> Handle(GetAllObstaclePositions message)
    {
        return new GetAllObstaclePositionsResponse(_obstacles.Keys.AsEnumerable());
    }
}