using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    [Header("Ammo")]
    #region Ammo
    [HideInInspector] public int clipSize; // Dimensione del caricatore
    [HideInInspector] public int extraAmmo; // Munizioni extra (escluse quelle nel caricatore)
    [HideInInspector] public int leftAmmo; // Munizioni attuali
    #endregion

    [Header("Audio properties")]
    #region AudioSource Settings
    public AudioClip magInSound; // Suono di inserimento del caricatore
    public AudioClip magOutSound; // Suono di estrazione del caricatore
    public AudioClip releaseSlideSound; // Suono di rilascio del caricatore
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Ammo ammoData; // Dati delle munizioni
    #endregion

    void Start()
    {
        if (ammoData == null) return;
        leftAmmo = ammoData.nAmmo;
        extraAmmo = ammoData.maxAmmo - ammoData.nAmmo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) Reload();
    }

    public void Reload()
    {
        if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - leftAmmo;
            extraAmmo -= ammoToReload;
            leftAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            if (extraAmmo + leftAmmo > clipSize)
            {
                int leftOverAmmo = extraAmmo + leftAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                leftAmmo = clipSize;
            }
            else
            {
                leftAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }
}
