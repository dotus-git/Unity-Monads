using Monads;
using TMPro;
using UniMediator;
using UnityEngine;
using UnityUtils;

public class TargetSystem :
    Singleton<TargetSystem>,
    ISingleMessageHandler<TargetPosition, Result<TargetPositionResponse>>,
    IMulticastMessageHandler<TargetSpotted>
{
    private const string DefaultTargetName = "-";
    
    public GameObject Cursor;
    
    public TextMeshProUGUI TargetNameText;
    public TextMeshProUGUI TargetTypeText;

    private void Start()
    {
        Cursor = Instantiate(Cursor);
    }

    public Result<TargetPositionResponse> Handle(TargetPosition message)
    {
        // var obstacleDetected = Mediator.Send(new DetectObstacle(message.Position));
        // if (obstacleDetected)
        // {
        //     return new TargetPositionResponse(obstacleDetected.SuccessValue, TargetType.Obstacle);
        // }
        //
        // var lootDetected = Mediator.Send(new DetectLoot(message.Position));
        // if (lootDetected)
        // {
        //     return new TargetPositionResponse(lootDetected.SuccessValue, TargetType.Loot);
        // }
        //
        // var unitDetected = Mediator.Send(new DetectUnit(message.Position));
        // if (unitDetected)
        // {
        //     return new TargetPositionResponse(unitDetected.SuccessValue, TargetType.Unit);
        // }

        return NotFound.Failure.ToResult<TargetPositionResponse>();
    }

    public void Handle(TargetSpotted message)
    {
        Cursor.transform.position = message.Position.ToV3();
        
        if (message.Type == TargetType.None || !message.Target)
        {
            TargetNameText.text = DefaultTargetName;
            TargetTypeText.text = DefaultTargetName;
            return;
        }
        
        var id = message.Target.GetComponent<Info>();
        var targetName = id
            ? id.Name
            : message.Target.name; 
        
        TargetNameText.text = targetName;
        TargetTypeText.text = message.Type.ToString();
    }
}