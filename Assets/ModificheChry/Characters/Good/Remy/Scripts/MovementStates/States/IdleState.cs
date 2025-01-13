using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement) { movement.noiseAura.radius = 1f; }

    public override void UpdateState(MovementStateManager movement)
    {
        if (movement.moveDirection.magnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift)) movement.SwitchState(movement.runningState);
            else movement.SwitchState(movement.walkingState);
        }
        if (Input.GetKeyDown(KeyCode.C)) movement.SwitchState(movement.crouchState);
    }
}
