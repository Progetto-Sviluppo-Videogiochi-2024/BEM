using DialogueEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Status")]
    #region Player Status
    [HideInInspector] public int maxHealth = 100; // Salute massima
    public int health; // Salute attuale
    [HideInInspector] public int sanitaMentale; // TODO: da cambiare // Salute mentale
    [HideInInspector] public bool isDead = false; // Stato del giocatore (vivo/morto)
    #endregion

    [Header("References")]
    #region References
    public NPCConversation conversation; // Conversazione quando il player prova a uscire dal campo di gioco con il fucile
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento al componente WeaponClassManager
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private RagdollManager ragdollManager; // Riferimento al componente RagdollManager
    #endregion

    void Start()
    {
        sanitaMentale = maxHealth;
        health = maxHealth;
        weaponClassManager = GetComponent<WeaponClassManager>();
        ragdollManager = GetComponent<RagdollManager>();
    }

    private void Update()
    {
        playerUIController.UpdateItemUI();
        var ammo = weaponClassManager.actions.weaponAmmo;
        if (ammo == null) return;
        playerUIController.extraAmmo = ammo.extraAmmo;
        playerUIController.UpdateWeaponUI();
        playerUIController.UpdateAmmoCount(ammo.currentAmmo);

        if (health <= sanitaMentale) // TODO: da implementare
        { }
    }

    public void UpdateHealth(int amount)
    {
        if (IsDead()) return; // Se è morto, non fare nulla

        health += amount;
        // TODO: aggiungere modifiche alla sanitaMentale
        if (health >= maxHealth) { health = maxHealth; return; }
        playerUIController.UpdateBloodSplatter(health, maxHealth);
        playerUIController.UpdateSanityIcon();
    }

    private bool IsDead()
    {
        if (isDead) return true; // Se era già morto, non fare nulla

        if (health <= 0) // Se è appena morto
        {
            // ragdollManager.TriggerRagdoll();
            health = 0;
            isDead = true;
            return true;
        }
        return false;
    }

    private bool CanReadDE() =>
        BooleanAccessor.istance.GetBoolFromThis("cocaCola") && // Se ho già parlato con UB della task fucile
        !BooleanAccessor.istance.GetBoolFromThis("cocaColaDone") && // Se non ho ancora completato la task fucile
        ConversationManager.Instance.hasClickedEnd; // Se ho finito la conversazione con UB (per evitare che si ripeta a ogni collisione col muro)

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall") && CanReadDE())
        {
            ConversationManager.Instance.StartConversation(conversation);
        }
    }
}
