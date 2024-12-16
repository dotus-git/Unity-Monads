using Monads;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        DataMediator.Instance
            .Send<RegisterObstacle, Result>(new RegisterObstacle(gameObject))
            .OnFailure(fail => Debug.LogError($"{fail} - Failed to register obstacle {name} at {transform.position}"));

        // Mediator
        //     .Send(new RegisterObstacle(gameObject))
        //     .OnFailure(fail => Debug.LogError($"{fail} - Failed to register obstacle {name} at {transform.position}"));
    }
}
