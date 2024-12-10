using UniMediator;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int Score = 1;
    public int Gold = 1;
    
    private void Start()
    {
        Mediator.Send(new RegisterLoot(gameObject));
    }
}
