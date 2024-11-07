using UnityEngine;

public class RunningState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement) => movement.animator.SetBool("running", true);

    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKeyUp(KeyCode.LeftShift)) ExitState(movement, movement.walkingState);
        else if (movement.moveDirection.magnitude < 0.1f) ExitState(movement, movement.idleState);

        movement.currentMoveSpeed = movement.v < 0 ? movement.runBackSpeed : movement.runSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState newState)
    {
        movement.animator.SetBool("running", false);
        movement.SwitchState(newState);
    }
}
