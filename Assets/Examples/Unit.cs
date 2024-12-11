using UniMediator;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int ActionPoints;

    private void Start()
    {
        Mediator.Send(new RegisterUnit(gameObject));
    }
}
