using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultState : ActionBaseState
{
    public float scrollDirection;

    public override void EnterState(ActionStateManager actions) { }

    public override void UpdateState(ActionStateManager actions)
    {
        if (CanReload(actions) && Input.GetKeyDown(KeyCode.R))
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
        if (action.currentWeapon == null) return false; // Se non ha un'arma equipaggiata
        else if (action.weaponAmmo == null) return false; // Se l'arma equipaggiata non ha un ammo
        else if (action.weaponAmmo.currentAmmo == action.weaponAmmo.clipSize) return false; // Se il caricatore è pieno
        else if (action.weaponAmmo.extraAmmo == 0) return false; // Se non ha più munizioni
        else if (!action.animator.GetBool("hasFireWeapon")) return false; // Se non ha l'arma (equipaggiata ma non in mano)
        // else if è stordito return false;
        // else if sta salendo o scendendo le scale verticali return false; // se le metteremo in scena3
        // else if sta sparando return false; idk...
        // else if sta già ricaricando return false; // in realtà non lo fa quindi inutile metterlo
        else return true;
    }

    private bool CanSwap() =>
        Input.mouseScrollDelta.y != 0 // Se si sta scrollando
            && InventoryManager.instance.weaponsEquipable.Count > 1 // Se ne ha almeno due equipaggiate
            && !EventSystem.current.IsPointerOverGameObject(); // Se il mouse non è sopra una UI
}
