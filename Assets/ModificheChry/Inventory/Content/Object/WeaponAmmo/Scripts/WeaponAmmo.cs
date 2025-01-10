using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo")]
    #region Ammo
    [HideInInspector] public int clipSize; // Dimensione del caricatore (un caricatore ha 'clipsize' munizioni)
    [HideInInspector] public int extraAmmo; // Munizioni extra (escluse quelle nel caricatore)
    [HideInInspector] public int currentAmmo; // Munizioni attuali nel caricatore corrente
    #endregion

    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool isLoadingSlot; // Flag indicante se il load slot è da caricare
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Ammo data; // Dati delle munizioni
    #endregion

    void Start()
    {
        if (data == null) return;
        clipSize = data.nAmmo;
        if (!isLoadingSlot) currentAmmo = data.maxAmmo;
    }

    public void UpdateAmmo(Ammo ammo, bool isAmmo)
    {
        var ammoToAdd = isAmmo ? ammo.ammoToAdd : ammo.qta * ammo.ammoToAdd;
        data = ammo;
        extraAmmo += ammoToAdd;
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
