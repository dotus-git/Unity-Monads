using System.Collections.Generic;
using Dotus.Core;
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
    ISingleMessageHandler<DetectLoot, Result<GameObject>>
{
    private static readonly Dictionary<Vector2Int, GameObject> Loot = new();

    public TextMeshProUGUI GoldText; // Assign this in the inspector

    private int gold;

    private void Start()
        => GoldText
            .ToResult()
            .Do(ui => ui.text = gold.ToString());

    public void Handle(AddGold message)
        => GoldText
            .ToResult()
            .Do(text => {
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
            .Do(Destroy);

    public Result<GameObject> Handle(DetectLoot message)
        => Loot.TryGetValue(message.Position, out var loot)
            ? loot.ToResult()
            : Failure.Default;

    public Result Handle(RegisterLoot message)
        => Loot.TryAdd(message.Unit.transform.position.ToV2I(), message.Unit).ToResult();

    private static Result<GameObject> PopLoot(Vector2Int position)
        => Loot.Remove(position, out var lootObject)
            ? lootObject.ToResult()
            : Failure.Default;

    public void Handle(NewGame message)
    {
        gold = 0;
    }
}