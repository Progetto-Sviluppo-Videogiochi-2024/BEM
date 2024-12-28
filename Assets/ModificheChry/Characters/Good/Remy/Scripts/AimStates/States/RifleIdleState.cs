using UnityEngine;

public class RifleIdleState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim.animator.SetBool("aiming", false);
        aim.currentFov = aim.idleFov;
        if (aim.weaponClassManager.currentWeaponIndex != -1) aim.weaponClassManager.weaponsEquipable[aim.weaponClassManager.currentWeaponIndex].SetIdle();
        aim.crosshair.SetActive(false);
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKey(KeyCode.Mouse1)) aim.SwitchState(aim.rifleIdleAimState); // Destro del mouse
    }
}
