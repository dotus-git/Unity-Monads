using System.Collections.Generic;
using Monads;
using UniMediator;
using UnityEngine;

public readonly struct GetAllLootPositions : ISingleMessage<Result<GetAllLootPositionsResponse>> {}

public readonly struct RegisterLoot : ISingleMessage<Result>
{
    public readonly GameObject Unit;

    public RegisterLoot(GameObject unit)
    {
        Unit = unit;
    }
}

public readonly struct GetAllLootPositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllLootPositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}

public readonly struct DetectLoot : ISingleMessage<Result<GameObject>>
{
    public readonly Vector2Int Position;

    public DetectLoot(Vector2Int position)
    {
        Position = position;
    }
}

public readonly struct GetLoot : IMulticastMessage
{
    public readonly GameObject Unit;
    public readonly GameObject Loot;

    public GetLoot(GameObject unit, GameObject loot)
    {
        Unit = unit;
        Loot = loot;
    }
}