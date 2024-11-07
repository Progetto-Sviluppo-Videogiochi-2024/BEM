using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultState : ActionBaseState
{
    public float scrollDirection;

    public override void EnterState(ActionStateManager actions)
    {
        actions.rightHandAim.weight = 1;
        actions.leftHandIK.weight = 1;
    }

    public override void UpdateState(ActionStateManager actions)
    {
        actions.rightHandAim.weight = Mathf.Lerp(actions.rightHandAim.weight, 1, Time.deltaTime * 10);
        if (actions.leftHandIK.weight == 0) actions.leftHandIK.weight = 1;

        if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
        {
            actions.SwitchState(actions.reloadState);
        }
        else if (CanSwap())
        {
            scrollDirection = Input.mouseScrollDelta.y;
            actions.SwitchState(actions.swapState);
        }
    }

    private bool CanReload(ActionStateManager action)
    {
        if (action.weaponAmmo.leftAmmo == action.weaponAmmo.clipSize) return false;
        else if (action.weaponAmmo.extraAmmo == 0) return false;
        else return true;
    }

    private bool CanSwap()
    {
        return Input.mouseScrollDelta.y != 0 // Se si sta scrollando
            && InventoryManager.instance.weaponsEquipable.Count > 1 // Se ne ha almeno due equipaggiate
            && !EventSystem.current.IsPointerOverGameObject(); // Se il mouse non è sopra un UI
    }
}
