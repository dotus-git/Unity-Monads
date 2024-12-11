using System.Collections.Generic;
using System.Linq;
using Monads;
using UniMediator;
using UnityEngine;
using UnityUtils;
using static ActionPointConstants;

public class UnitSystem :
    Singleton<UnitSystem>,
    ISingleMessageHandler<RegisterUnit, Result>,
    ISingleMessageHandler<MoveUnit, Result<MoveUnitResponse>>,
    ISingleMessageHandler<DetectUnit, Result<GameObject>>,
    ISingleMessageHandler<GetAllUnitPositions, Result<GetAllUnitPositionsResponse>>
{
    private readonly Dictionary<Vector2Int, GameObject> _units = new();

    public Result<MoveUnitResponse> Handle(MoveUnit message)
    {
        var obstacleDetected = Mediator.Send(new DetectObstacle(message.Destination));
        if (obstacleDetected)
            return new PathBlocked(message.Destination);
        
        var lootDetected = Mediator.Send(new DetectLoot(message.Destination));
        if (lootDetected)
        {
            Mediator.Publish(new GetLoot(
                unit: message.Unit, 
                loot: lootDetected.SuccessValue));
            
            return new MoveUnitResponse(
                landingPosition: message.Destination,
                actionPoints: AP_PICKUP_LOOT);
        }
        
        var unitDetected = Mediator.Send(new DetectUnit(message.Destination));
        if (unitDetected)
        {
            //TODO: attack other unit, because we moved into their grid position

            return new PathBlocked(message.Destination);
        }

        return new MoveUnitResponse(
            landingPosition: message.Destination,
            actionPoints: AP_MOVE_ONE);
    }

    public Result<GameObject> Handle(DetectUnit message)
        => _units.TryGetValue(message.Position, out var unit)
            ? unit.ToResult()
            : Failure.Default;
    
    public Result Handle(RegisterUnit message)
        => _units.TryAdd(message.Unit.transform.position.ToV2I(), message.Unit).ToResult();

    public Result<GetAllUnitPositionsResponse> Handle(GetAllUnitPositions message)
        => new GetAllUnitPositionsResponse(Instance._units.Keys.AsEnumerable());
}