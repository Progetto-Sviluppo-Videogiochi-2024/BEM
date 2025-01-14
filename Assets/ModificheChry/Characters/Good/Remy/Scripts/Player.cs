using System.Collections;
using System.Collections.Generic;
using DialogueEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Status")]
    #region Player Status
    [HideInInspector] public bool IsLoading = false; // Flag per il caricamento dei dati, lo si usa nel caricamento dati e cambio scena
    [HideInInspector] public int maxHealth = 100; // Salute massima
    public int health; // Salute attuale
    public int sanitaMentale = 100; // Salute mentale
    [HideInInspector] public bool isDead = false; // Stato del giocatore (vivo/morto)
    [HideInInspector] public bool menteSana = true; // Stato della salute mentale ("sano"/"malato")
    [HideInInspector] public bool hasEnemyDetectedPlayer = false; // Flag per il rilevamento del giocatore da parte dei nemici
    #endregion

    [Header("References")]
    #region References
    public List<NPCConversation> conversations; // Lista di possibili conversazioni che l'utente può avere
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento al componente WeaponClassManager
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private RagdollManager ragdollManager; // Riferimento al componente RagdollManager
    public GameOverMenuManager gameOverMenuManager; // Riferimento al componente GameOverMenuManager
    private CharacterController controller; // Riferimento al CharacterController
    #endregion

    [Header("Audio Settings")]
    #region Audio Settings
    [SerializeField] private AudioSource breathingSource; // Riferimento all'AudioSource
    [SerializeField] private AudioSource hitSource; // Riferimento all'AudioSource
    [SerializeField] private AudioClip clipRespiro; // Clip audio per il respiro
    [SerializeField] private AudioClip audioClipColpito; // Audio che viene riprodotto quando il personaggio viene colpito    
    #endregion


    void Start()
    {
        // Init delle variabili
        breathingSource = gameObject.AddComponent<AudioSource>();
        hitSource = gameObject.AddComponent<AudioSource>();
        weaponClassManager = GetComponent<WeaponClassManager>();
        ragdollManager = GetComponent<RagdollManager>();
        controller = GetComponent<CharacterController>();

        // Aggiorna l'UI del giocatore (anche se non ha subito danni -> per il caricamento dei dati)
        UpdateStatusPlayer(0, 0);
    }

    void Update()
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
        if (isDead) return;

        health += amountHealth;
        sanitaMentale += amountSanita;
        if (amountHealth < 0)
        {
            sanitaMentale -= 10;
            PlayHitSound(); // Riproduci il suono quando subisce danni
        }
        if (health <= 20) sanitaMentale -= 15;

        menteSana = sanitaMentale > 40;
        if (!menteSana) PlayBreathing();
        else breathingSource.Stop();

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
            if (breathingSource != null && breathingSource.isPlaying)
            {
                breathingSource.Stop(); // Ferma l'audio solo se è in riproduzione
            }
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
        yield return new WaitForSeconds(3.5f);
        gameOverMenuManager.ToggleMenu(true);
    }

    private bool CanReadDE() =>
        BooleanAccessor.istance.GetBoolFromThis("cocaCola") && // Se ho già parlato con UB della task fucile
        !BooleanAccessor.istance.GetBoolFromThis("cocaColaDone") && // Se non ho ancora completato la task fucile
        ConversationManager.Instance.hasClickedEnd; // Se ho finito la conversazione con UB (per evitare che si ripeta a ogni collisione col muro)

    private void PlayBreathing()
    {
        if (clipRespiro == null || breathingSource.isPlaying) return;  // Evita di riprodurre lo stesso audio se già in corso
        breathingSource.loop = true;
        breathingSource.clip = clipRespiro; // Usa la clip corretta
        breathingSource.Play(); // Riproduci il suono del respiro
    }

    private void PlayHitSound()
    {
        if (audioClipColpito != null)
        {
            hitSource.PlayOneShot(audioClipColpito); // Riproduci il suono di impatto
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // if (hit.collider.CompareTag("Mutant")) // Se è un mutante e se il player sta salendo o spingendo verso l'alto, fermalo e correggi la sua posizione
        // {
        //     print($"Player colpito da {hit.collider.name}, velocità {controller.velocity.y}");
        //     if (controller.velocity.y < 0f) transform.position = new(transform.position.x, hit.collider.bounds.max.y, transform.position.z);
        // }

        NPCConversation conversation = null;
        if (hit.gameObject.CompareTag("Wall") && CanReadDE()) conversation = conversations[0];
        if (hit.gameObject.CompareTag("WallConfine")) conversation = conversations[1];

        if (conversation != null) ConversationManager.Instance.StartConversation(conversation);
    }
}
