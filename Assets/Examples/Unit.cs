using Monads;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int ActionPoints;

    private void Start()
    {
        DataMediator.Instance.Send<RegisterUnit, Result>(new RegisterUnit(gameObject));
    }
}
