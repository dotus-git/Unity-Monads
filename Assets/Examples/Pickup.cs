using Monads;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public int Score = 1;
    public int Gold = 1;
    
    private void Start()
    {
        DataMediator.Instance.Send<RegisterLoot, Result>(new RegisterLoot(gameObject));
    }
}
