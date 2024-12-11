using System.Collections.Generic;
using System.Linq;
using Monads;
using TMPro;
using UniMediator;
using UnityEngine;
using UnityUtils;

// We can create entire systems where messages map to digestible expression-body methods
// This can make larger games easier to understand by a single or small group of devs
public class LootSystem :
    Singleton<UnitSystem>,
    IMulticastMessageHandler<GetLoot>,
    IMulticastMessageHandler<AddGold>,
    IMulticastMessageHandler<NewGame>,
    ISingleMessageHandler<RegisterLoot, Result>,
    ISingleMessageHandler<DetectLoot, Result<GameObject>>,
    ISingleMessageHandler<GetAllLootPositions, Result<GetAllLootPositionsResponse>>
{
    private readonly Dictionary<Vector2Int, GameObject> _loot = new();

    public TextMeshProUGUI GoldText; // Assign this in the inspector

    private int gold;

    private void Start()
        => GoldText
            .ToResult()
            .OnSuccess(ui => ui.text = gold.ToString());

    public void Handle(AddGold message)
        => GoldText
            .ToResult()
            .OnSuccess(text => {
                gold += message.Amount;
                text.text = gold.ToString();
            });

    public void Handle(GetLoot message)
        => PopLoot(message.Loot.transform.position.ToV2I())
            .WithSafeComponent<Pickup>(pickup => {
                if (pickup.Score > 0)
                    Mediator.Publish(new AddScore(pickup.Score));

                if (pickup.Gold > 0)
                    Mediator.Publish(new AddGold(pickup.Gold));
            })
            .OnSuccess(Destroy);

    public Result<GameObject> Handle(DetectLoot message)
        => _loot.TryGetValue(message.Position, out var loot)
            ? loot.ToResult()
            : Failure.Default;

    public Result Handle(RegisterLoot message)
        => _loot.TryAdd(message.Unit.transform.position.ToV2I(), message.Unit).ToResult();

    private Result<GameObject> PopLoot(Vector2Int position)
        => _loot.Remove(position, out var lootObject)
            ? lootObject.ToResult()
            : Failure.Default;

    public void Handle(NewGame message)
    {
        gold = 0;
    }

    public Result<GetAllLootPositionsResponse> Handle(GetAllLootPositions message)
        => new GetAllLootPositionsResponse(_loot.Keys.AsEnumerable());
}