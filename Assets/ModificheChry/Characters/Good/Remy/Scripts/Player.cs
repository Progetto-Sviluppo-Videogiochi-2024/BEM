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
    public NPCConversation conversation; // Conversazione quando il player prova a uscire dal campo di gioco con il fucile
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento al componente WeaponClassManager
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private RagdollManager ragdollManager; // Riferimento al componente RagdollManager
    private AudioSource audiosource;
    public AudioClip clip;
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
        //Ho aggiunto il controllo qui per avere un controllo attivo, 
        //in PlayBreathing o UpdateHealth il controllo sarebbe stato sporadico.
        //if(audiosource.isPlaying && audiosource.time >= 15.0f && menteSana == false)
        //{print("prova");}
        if (sanitaMentale <= 50)
        {
            health -= 1;
        }
    }

    public void UpdateHealth(int amount)
    {   // TODO: distinguere tipo di cura se per la salute o per la sanità mentale
        if (IsDead()) return; // Se è morto, non fare nulla
        // TODO: trovare un modo per distinguere la cura della sanità mentale da quella della salute
        health += amount;
        if (amount < 0) sanitaMentale -= 50;

        if (sanitaMentale <= 50) menteSana = false;

        if (menteSana == false) PlayBreathing();

        if (sanitaMentale > 50 && menteSana == false)
        {
            menteSana = true;
            PlayBreathing();
        }
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

    private void PlayBreathing()
    {
        if (audiosource == null)
        {
            audiosource = gameObject.AddComponent<AudioSource>();
            audiosource.loop = true;
            audiosource.clip = clip;
            audiosource.time = 5.0f;
        }
        if (audiosource.clip != null)
        {
            audiosource.Play();
            if (audiosource.isPlaying == false) audiosource.Play();
            else if (menteSana == true)
            {
                print("loop annullato");
                audiosource.loop = false;
            }
        }
        else { print("Non va l'audio"); }
    }
}
