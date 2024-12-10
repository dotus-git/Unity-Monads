using System.Collections.Generic;
using Dotus.Core;
using Monads;
using UniMediator;
using UnityEngine;
using UnityUtils;
using static ActionPointConstants;

public class UnitSystem :
    Singleton<UnitSystem>,
    ISingleMessageHandler<RegisterUnit, Result>,
    ISingleMessageHandler<MoveUnit, Result<MoveUnitResponse>>,
    ISingleMessageHandler<DetectUnit, Result<GameObject>>
{
    private static readonly Dictionary<Vector2Int, GameObject> Units = new();

    public Result<MoveUnitResponse> Handle(MoveUnit message)
    {
        var obstacleDetected = Mediator.Send(new DetectObstacle(message.Destination));
        if (obstacleDetected)
            return new PathBlocked(message.Destination);
        
        var lootDetected = Mediator.Send(new DetectLoot(message.Destination));
        if (lootDetected)
        {
            Mediator.Publish(new GetLoot(
                Unit: message.Unit, 
                Loot: lootDetected.SuccessValue));
            
            return new MoveUnitResponse(
                LandingPosition: message.Destination,
                ActionPoints: AP_PICKUP_LOOT);
        }
        
        var unitDetected = Mediator.Send(new DetectUnit(message.Destination));
        if (unitDetected)
        {
            // attack because we moved into another unit
        }

        return new MoveUnitResponse(
            LandingPosition: message.Destination,
            ActionPoints: AP_MOVE_ONE);
    }

    public Result<GameObject> Handle(DetectUnit message)
        => Units.TryGetValue(message.Position, out var unit)
            ? unit.ToResult()
            : Failure.Default;
    
    public Result Handle(RegisterUnit message)
        => Units.TryAdd(message.Unit.transform.position.ToV2I(), message.Unit).ToResult();
}