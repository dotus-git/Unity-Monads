using Dotus.Core;
using Monads;
using UniMediator;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

public class PlayerController : Singleton<PlayerController>
{
    public GameObject ControlledUnit;
    
    public float MinDelay = 0.15f;
    private float lastMove = float.MinValue;
    
    private Result PressedTwiceTooQuickly()
        => (Mathf.Abs(Time.time - lastMove) < MinDelay).ToResult();
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // one event per input-device user action
        if (context.phase != InputActionPhase.Performed)
            return;

        if (PressedTwiceTooQuickly())
            return;
        
        if (!ControlledUnit)
        {
            Debug.LogWarning("No controlled unit");
            return;
        }
        
        // read the value for the "move" action each event call
        var moveAmount = context.ReadValue<Vector2>().ToV2I();
        if (moveAmount == Vector2Int.zero)
            return;

        var newUnitPosition = ControlledUnit.transform.position.ToV2I() + moveAmount;
        Mediator.Send(new MoveUnit(ControlledUnit, newUnitPosition))
            .Do(response => {
                ControlledUnit.transform.position = response.LandingPosition.ToV3();
            });
        
        lastMove = Time.time;
    }
}
