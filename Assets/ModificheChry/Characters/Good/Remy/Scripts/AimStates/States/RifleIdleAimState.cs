
using UnityEngine;

public class RifleIdleAimState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim.animator.SetBool("aiming", true);
        aim.currentFov = aim.aimFov;
        aim.cambiaRig.SetAim();
        aim.crosshair.SetActive(true);
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKeyUp(KeyCode.Mouse1)) aim.SwitchState(aim.rifleIdleState); // Destro del mouse
    }
}
