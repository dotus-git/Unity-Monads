using UniMediator;

public readonly struct NewGame : IMulticastMessage {}

public readonly struct PlayerDead : IMulticastMessage {}

public readonly struct AddGold : IMulticastMessage
{
    public readonly int Amount;

    public AddGold(int amount)
    {
        Amount = amount;
    }
}

public readonly struct AddScore : IMulticastMessage
{
    public readonly int Amount;

    public AddScore(int amount)
    {
        Amount = amount;
    }
}