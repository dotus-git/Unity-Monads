using Monads;
using TMPro;
using UnityEngine;
using UnityUtils;

public class TargetSystem :
    Singleton<TargetSystem>
{
    private const string DefaultTargetName = "-";
    
    public GameObject Cursor;
    
    public TextMeshProUGUI TargetNameText;
    public TextMeshProUGUI TargetTypeText;

    private void Start()
    {
        Cursor = Instantiate(Cursor);
    }

    [MediatorHandler]
    public Result<TargetPositionResponse> Handle(TargetPosition message)
    {
        var obstacleDetected = DataMediator.Instance.Send<DetectObstacle, Result<GameObject>>(new DetectObstacle(message.Position));
        if (obstacleDetected)
        {
            return new TargetPositionResponse(obstacleDetected.SuccessValue, TargetType.Obstacle);
        }
        
        var lootDetected = DataMediator.Instance.Send<DetectLoot, Result<GameObject>>(new DetectLoot(message.Position));
        if (lootDetected)
        {
            return new TargetPositionResponse(lootDetected.SuccessValue, TargetType.Loot);
        }
        
        var unitDetected = DataMediator.Instance.Send<DetectUnit, Result<GameObject>>(new DetectUnit(message.Position));
        if (unitDetected)
        {
            return new TargetPositionResponse(unitDetected.SuccessValue, TargetType.Unit);
        }

        return NotFound.Failure.ToResult<TargetPositionResponse>();
    }

    [MediatorHandler]
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