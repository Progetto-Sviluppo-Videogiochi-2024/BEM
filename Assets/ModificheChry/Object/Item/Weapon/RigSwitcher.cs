using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigSwitcher : MonoBehaviour
{
    [Header("Weapon Rig Settings")]
    private GameObject currentWeapon; // L'arma attualmente equipaggiata
    Rig[] allRigs; 
    Rig currentRig; // Rig per lo stato aim

    [Header("Weapon Settings")]
    Weapon weapon;

    void Start()
    {
        allRigs = gameObject.GetComponentsInChildren<Rig>();
    }

    public void SetIdle()
    {
        string rigCercato = "RigIdle" + weapon.name;
        currentWeapon.transform.localPosition = weapon.IdlePosition;
        currentWeapon.transform.localRotation = weapon.IdleRotation;
         foreach (Rig rig in allRigs)
        { 
            if (rig.name == rigCercato)
            {
                if(currentRig!= null){ currentRig.weight=0;}
                rig.weight=1;
                currentRig=rig;
                break;
            }
        }
    }
    public void SetAim()
    {   
        string rigCercato = "RigIdle" + weapon.name;
        currentWeapon.transform.localPosition = weapon.AimPosition;
        currentWeapon.transform.localRotation = weapon.AimRotation;
        currentWeapon.transform.localScale = weapon.Scale;
        foreach (Rig rig in allRigs)
        {
            if (rig.name == rigCercato)
            {
                if(currentRig!= null){ currentRig.weight=0;}
                rig.weight=1;
                currentRig=rig;
                break;
            }
        }
    }

    public void SwitchWeapon(GameObject newWeapon)
    {
        currentWeapon = newWeapon; // Cambia l'arma corrente
        weapon = currentWeapon.GetComponent<WeaponManager>().weapon;
        SetIdle(); // Imposta la nuova arma a idle
    }
}
