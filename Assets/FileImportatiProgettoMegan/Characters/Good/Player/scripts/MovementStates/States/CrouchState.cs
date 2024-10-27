using UnityEngine;

public class CrouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement) => movement.animator.SetBool("crouching", true);

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.runningState);
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (movement.moveDirection.magnitude < 0.1f) ExitState(movement, movement.idleState);
            else ExitState(movement, movement.walkingState);
        }

        movement.currentMoveSpeed = movement.v < 0 ? movement.crouchBackSpeed : movement.crouchSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState newState)
    {
        movement.animator.SetBool("crouching", false);
        movement.SwitchState(newState);
    }
}
