using System.Collections.Generic;
using UnityEngine;

[MediatorMessage]
public readonly struct GetAllLootPositions {}

[MediatorMessage]
public readonly struct RegisterLoot 
{
    public readonly GameObject Unit;

    public RegisterLoot(GameObject unit)
    {
        Unit = unit;
    }
}

[MediatorMessage]
public readonly struct GetAllLootPositionsResponse
{
    public readonly IEnumerable<Vector2Int> Points;

    public GetAllLootPositionsResponse(IEnumerable<Vector2Int> points)
    {
        Points = points;
    }
}

[MediatorMessage]
public readonly struct DetectLoot 
{
    public readonly Vector2Int Position;

    public DetectLoot(Vector2Int position)
    {
        Position = position;
    }
}

[MediatorMessage]
public readonly struct GetLoot 
{
    public readonly GameObject Unit;
    public readonly GameObject Loot;

    public GetLoot(GameObject unit, GameObject loot)
    {
        Unit = unit;
        Loot = loot;
    }
}