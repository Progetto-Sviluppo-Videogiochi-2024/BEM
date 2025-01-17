using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo")]
    #region Ammo
    [HideInInspector] public int clipSize; // Dimensione del caricatore (un caricatore ha 'clipsize' munizioni)
    [HideInInspector] public int extraAmmo; // Munizioni extra (escluse quelle nel caricatore)
    [HideInInspector] public int currentAmmo; // Munizioni attuali nel caricatore corrente
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Ammo data; // Dati delle munizioni
    #endregion

    public void InitAmmo() // Quando raccolgo l'arma per la prima volta l'arma
    {
        if (data == null) return;
        clipSize = data.nAmmo;
        currentAmmo = data.maxAmmo;
        extraAmmo = 0;
    }

    public void UpdateAmmo(Ammo ammo, bool isAmmo) // Quando raccolgo munizioni o armi
    {
        var ammoToAdd = isAmmo ? ammo.ammoToAdd : ammo.qta * ammo.ammoToAdd;
        data ??= ammo;
        extraAmmo += ammoToAdd;
    }

    public void SetAmmo(Weapon weapon) // Quando carico lo slot di salvataggio
    {
        currentAmmo = weapon.currentAmmo;
        extraAmmo = weapon.extraAmmo;
        clipSize = data.nAmmo;
    }

    public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            if (extraAmmo + currentAmmo > clipSize)
            {
                int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                currentAmmo = clipSize;
            }
            else
            {
                currentAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }
}
