using System.Collections;
using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Status")]
    #region Player Status
    [HideInInspector] public int maxHealth = 100; // Salute massima
    public int health; // Salute attuale
    public int sanitaMentale; // Salute mentale
    [HideInInspector] public bool isDead = false; // Stato del giocatore (vivo/morto)
    [HideInInspector] public bool menteSana = true; // Stato della salute mentale ("sano"/"malato")
    #endregion

    [Header("References")]
    #region References
    public List<NPCConversation> conversations; // Lista di possibili conversazioni che l'utente può avere
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento al componente WeaponClassManager
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private RagdollManager ragdollManager; // Riferimento al componente RagdollManager
    private AudioSource audiosource; // Riferimento all'AudioSource
    public AudioClip clip; // Clip audio per il respiro
    public GameOverMenuManager gameOverMenuManager; // Riferimento al componente GameOverMenuManager
    #endregion

    void Start()
    {
        sanitaMentale = maxHealth;
        menteSana = true;
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
    }

    public void UpdateStatusPlayer(int amountHealth, int amountSanita)
    {   // TODO: distinguere tipo di cura se per la salute o per la sanità mentale  // TODO: trovare un modo per distinguere la cura della sanità mentale da quella della salute
        if (isDead) return; // Se è morto, non fare nulla

        health += amountHealth;
        sanitaMentale += amountSanita;
        if (amountHealth < 0) sanitaMentale -= 10;
        if (health <= 20) sanitaMentale -= 15;

        if (sanitaMentale <= 40) menteSana = false;

        if (!menteSana) PlayBreathing();

        if (sanitaMentale > maxHealth / 2 && !menteSana)
        {
            menteSana = true;
            PlayBreathing();
        }
        if (health >= maxHealth) health = maxHealth;
        if (sanitaMentale < 0) sanitaMentale = 0;

        playerUIController.UpdateBloodSplatter(health, maxHealth);
        playerUIController.UpdateSanityIcon(sanitaMentale, maxHealth);

        if (IsDead()) return;
    }

    private bool IsDead()
    {
        if (health <= 0) // Se è appena morto
        {
            Ragdoll();
            StartCoroutine(TimeoutToGameOver());
            health = 0;
            isDead = true;
            return true;
        }
        return false;
    }

    private void Ragdoll()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<MovementStateManager>().enabled = false;
        ragdollManager.TriggerRagdoll();
    }

    private IEnumerator TimeoutToGameOver()
    {
        yield return new WaitForSeconds(4f);
        gameOverMenuManager.ToggleMenu(true);
    }

    private bool CanReadDE() =>
        BooleanAccessor.istance.GetBoolFromThis("cocaCola") && // Se ho già parlato con UB della task fucile
        !BooleanAccessor.istance.GetBoolFromThis("cocaColaDone") && // Se non ho ancora completato la task fucile
        ConversationManager.Instance.hasClickedEnd; // Se ho finito la conversazione con UB (per evitare che si ripeta a ogni collisione col muro)

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        NPCConversation conversation = null;
        if (hit.gameObject.CompareTag("Wall") && CanReadDE()) conversation = conversations[0];
        if (hit.gameObject.CompareTag("WallConfine")) conversation = conversations[1];

        if (conversation != null) ConversationManager.Instance.StartConversation(conversation);
    }

    private void PlayBreathing()
    {
        if (audiosource == null)
        {
            audiosource = gameObject.AddComponent<AudioSource>();
            audiosource.loop = true;
            audiosource.clip = clip;
        }
        if (audiosource.clip != null)
        {
            audiosource.Play();
            if (audiosource.isPlaying == false) audiosource.Play();
            else if (menteSana == true) audiosource.loop = false;
        }
        else { print("Non va l'audio"); }
    }
}
