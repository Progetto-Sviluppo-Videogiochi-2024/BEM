using UnityEngine;

public class WalkingState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement) => movement.animator.SetBool("walking", true);

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.runningState);
        else if (Input.GetKeyDown(KeyCode.C)) ExitState(movement, movement.crouchState);
        else if (movement.moveDirection.magnitude < 0.1f) ExitState(movement, movement.idleState);

        movement.currentMoveSpeed = movement.v < 0 ? movement.walkBackSpeed : movement.walkSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState newState)
    {
        movement.animator.SetBool("walking", false);
        movement.SwitchState(newState);
    }
}
