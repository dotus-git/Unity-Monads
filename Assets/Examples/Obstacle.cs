using Monads;
using UniMediator;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        Mediator
            .Send(new RegisterObstacle(gameObject))
            .OnFailure(fail => Debug.LogError($"{fail} - Failed to register obstacle {name} at {transform.position}"));
    }
}
