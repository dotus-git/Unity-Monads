[MediatorMessage]
public readonly struct NewGame { }

[MediatorMessage]
public readonly struct PlayerDead { }

[MediatorMessage]
public readonly struct AddGold
{
    public readonly int Amount;

    public AddGold(int amount)
    {
        Amount = amount;
    }
}

[MediatorMessage]
public readonly struct AddScore
{
    public readonly int Amount;

    public AddScore(int amount)
    {
        Amount = amount;
    }
}