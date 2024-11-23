using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Status")]
    #region Player Status
    [HideInInspector] public int maxHealth = 100; // Salute massima
    public int health; // Salute attuale
    public int sanita_mentale = 100; // TODO: da cambiare // Salute mentale
    [HideInInspector] public bool isDead = false; // Stato del giocatore (vivo/morto)
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento al componente WeaponClassManager
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private RagdollManager ragdollManager; // Riferimento al componente RagdollManager
    #endregion

    void Start()
    {
        health = maxHealth;
        weaponClassManager = GetComponent<WeaponClassManager>();
        ragdollManager = GetComponent<RagdollManager>();
    }

    private void Update()
    {
        var ammo = weaponClassManager.actions.weaponAmmo;
        if (ammo == null) return;
        playerUIController.extraAmmo = ammo.extraAmmo;
        playerUIController.UpdateWeaponUI();
        playerUIController.UpdateAmmoCount(ammo.leftAmmo);

        if (health <= sanita_mentale) // TODO: da implementare
        { }
    }

    public void UpdateHealth(int amount)
    {
        if (IsDead()) return; // Se è morto, non fare nulla
        health += amount;
        playerUIController.UpdateBloodSplatter(health, maxHealth);
        playerUIController.UpdateSanityIcon();
    }

    private bool IsDead()
    {
        if (isDead) return true; // Se era già morto, non fare nulla

        if (health <= 0) // Se è appena morto
        {
            ragdollManager.TriggerRagdoll();
            health = 0;
            isDead = true;
            return true;
        }
        return false;
    }
}
