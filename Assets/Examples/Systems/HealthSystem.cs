using UniMediator;
using UnityUtils;

public class HealthSystem : 
    Singleton<HealthSystem>,
    IMulticastMessageHandler<NewGame>
{
    public const int DEFAULT_MAX_HEALTH = 3;
    
    public int MaxHealth = DEFAULT_MAX_HEALTH;
    public float CurrentHealth = DEFAULT_MAX_HEALTH;
    
    public void Handle(NewGame message)
    {
        MaxHealth = DEFAULT_MAX_HEALTH;
    }
}