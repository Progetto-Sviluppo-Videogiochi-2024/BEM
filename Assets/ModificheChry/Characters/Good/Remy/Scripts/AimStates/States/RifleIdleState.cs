using UnityEngine;

public class RifleIdleState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim.animator.SetBool("aiming", false);
        aim.currentFov = aim.idleFov;
       if(aim.CambiaRig!=null) { aim.CambiaRig.SetIdle();}
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKey(KeyCode.Mouse1)) aim.SwitchState(aim.rifleIdleAimState); // Destro del mouse
        
    }
}
