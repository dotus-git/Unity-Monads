using Monads;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

public class PlayerController : Singleton<PlayerController>
{
    public Camera MainCamera;
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
        DataMediator.Instance
            .Send<MoveUnit, Result<MoveUnitResponse>>(new MoveUnit(ControlledUnit, newUnitPosition))
            .OnSuccess(response => {
                ControlledUnit.transform.position = response.LandingPosition.ToV3();
            });

        lastMove = Time.time;
    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        // one event per input-device user action
        if (context.phase != InputActionPhase.Performed)
            return;

        var mousePosition = context.ReadValue<Vector2>();
        var worldPosition = MainCamera.ScreenToWorldPoint(mousePosition).ToV2IRound();

        // avoiding fluent chaining here to 
        // avoid anonymous functions and garbage
        var result = DataMediator.Instance.Send<TargetPosition, Result<TargetPositionResponse>>(new TargetPosition(worldPosition));
        
        if (result)
        {
            DataMediator.Instance.Publish(new TargetSpotted(
                position: worldPosition,
                target: result.SuccessValue.Target,
                type: result.SuccessValue.Type));
            // Mediator.Publish(new TargetSpotted(
            //     position: worldPosition,
            //     target: result.SuccessValue.Target,
            //     type: result.SuccessValue.Type));
        }
        else
        {
            DataMediator.Instance.Publish(new TargetSpotted(
                position: worldPosition,
                target: null,
                type: TargetType.None));
            // Mediator.Publish(new TargetSpotted(
            //     position: worldPosition,
            //     target: null,
            //     type: TargetType.None));
        }
    }
}