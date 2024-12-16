using System.Collections.Generic;
using System.Linq;
using Monads;
using UnityEngine;

public class LootSpawn : MonoBehaviour
{
    public List<LootSpawnInfo> LootTable;
    public GameArea GameArea;
    
    private readonly List<Vector2Int> _occupiedPositions = new();

    private void Start()
    {
        DataMediator.Instance
            .Send<GetAllObstaclePositions, Result<GetAllObstaclePositionsResponse>>(new GetAllObstaclePositions())
            .OnSuccess(response => _occupiedPositions.AddRange(response.Points));
        DataMediator.Instance
            .Send<GetAllUnitPositions, Result<GetAllUnitPositionsResponse>>(new GetAllUnitPositions())
            .OnSuccess(response => _occupiedPositions.AddRange(response.Points))
            ;

        PutCoinsInCorners();

        foreach (var loot in LootTable)
        {
            for (var i = 0; i < loot.Count; i++)
            {
                var lootObject = Instantiate(loot.Item, transform.position, Quaternion.identity);
                lootObject.transform.SetParent(transform);
                lootObject.transform.localPosition = GameArea.Area.GetRandomPosition(_occupiedPositions).ToV3();
            }
        }
    }

    private void PutCoinsInCorners()
    {
        var loot = LootTable.First();
        
        var lootObject = Instantiate(loot.Item, transform.position, Quaternion.identity);
        lootObject.transform.SetParent(transform);
        lootObject.transform.localPosition = GameArea.Area.min.ToV3();
        
        lootObject = Instantiate(loot.Item, transform.position, Quaternion.identity);
        lootObject.transform.SetParent(transform);
        lootObject.transform.localPosition = new Vector3(GameArea.Area.min.x, GameArea.Area.max.y, 0);
        
        lootObject = Instantiate(loot.Item, transform.position, Quaternion.identity);
        lootObject.transform.SetParent(transform);
        lootObject.transform.localPosition = new Vector3(GameArea.Area.max.x, GameArea.Area.min.y, 0);
        
        lootObject = Instantiate(loot.Item, transform.position, Quaternion.identity);
        lootObject.transform.SetParent(transform);
        lootObject.transform.localPosition = GameArea.Area.max.ToV3();
    }
}