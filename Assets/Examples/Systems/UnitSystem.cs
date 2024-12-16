using System.Collections.Generic;
using System.Linq;
using Monads;
using UnityEngine;
using UnityUtils;
using static ActionPointConstants;

public class UnitSystem :
    Singleton<UnitSystem>
{
    private readonly Dictionary<Vector2Int, GameObject> _units = new();

    [MediatorHandler]
    public Result<MoveUnitResponse> Handle(MoveUnit message)
    {
        var obstacleDetected = DataMediator.Instance.Send<DetectObstacle, Result<GameObject>>(new DetectObstacle(message.Destination));
        if (obstacleDetected)
            return new PathBlocked(message.Destination);
        
        var lootDetected = DataMediator.Instance.Send<DetectLoot, Result<GameObject>>(new DetectLoot(message.Destination));
        if (lootDetected)
        {
            DataMediator.Instance.Publish(new GetLoot(
                unit: message.Unit, 
                loot: lootDetected.SuccessValue));
            
            return new MoveUnitResponse(
                landingPosition: message.Destination,
                actionPoints: AP_PICKUP_LOOT);
        }
        
        var unitDetected = DataMediator.Instance.Send<DetectUnit, Result<GameObject>>(new DetectUnit(message.Destination));
        if (unitDetected)
        {
            //TODO: attack other unit, because we moved into their grid position

            return new PathBlocked(message.Destination);
        }

        return new MoveUnitResponse(
            landingPosition: message.Destination,
            actionPoints: AP_MOVE_ONE);
    }

    [MediatorHandler]
    public Result<GameObject> Handle(DetectUnit message)
        => _units.TryGetValue(message.Position, out var unit)
            ? unit.ToResult()
            : Failure.Default;
    
    [MediatorHandler]
    public Result Handle(RegisterUnit message)
        => _units.TryAdd(message.Unit.transform.position.ToV2I(), message.Unit).ToResult();

    [MediatorHandler]
    public Result<GetAllUnitPositionsResponse> Handle(GetAllUnitPositions message)
        => new GetAllUnitPositionsResponse(Instance._units.Keys.AsEnumerable());
}