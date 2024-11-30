using DialogueEditor;
using UnityEngine;

public class TaskFucile : MonoBehaviour
{
    [Header("References Scripts")]
    #region References Scripts
    private BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    private ConversationManager conversationManager; // Riferimento al ConversationManager
    #endregion

    [Header("References")]
    #region References
    private Weapon weaponClone; // Riferimento al fucile clonato
    public GameObject fucile; // Riferimento al fucile
    public Transform player; // Riferimento al player
    public NPCConversation[] conversations; // Conversazioni con l'uomo baita per il tutorial del fucile
    [HideInInspector] public GameObject areaTask; // Riferimento a dove il player può raccogliere il fucile e iniziare il tutorial
    #endregion

    [Header("Settings")]
    #region Settings
    private bool isConversationActive = false; // Indica se la conversazione è attiva
    private bool weaponHandled = false; // Indica se l'arma è stata gestita
    [HideInInspector] public bool playerInRange = false; // Indica se è vicino al tronco
    private const int maxTargetHit = 3; // Numero di bottiglie da colpire per completare il tutorial
    #endregion

    void Start()
    {
        // P.S.: lascio l'aura attiva per far capire che è interagibile solo dopo la quest del lupo
        conversationManager = ConversationManager.Instance;
        booleanAccessor = BooleanAccessor.istance;
        fucile.GetComponent<ItemPickup>().enabled = false;
        areaTask = transform.GetChild(0).gameObject;
        areaTask.SetActive(false);
        ResetPlayerProgress();
    }

    void Update()
    {
        // P.S.: controlli preliminari per poter attivare/disattivare il tutorial del fucile
        if (booleanAccessor.GetBoolFromThis("cocaColaDone")) { enabled = false; return; }
        if (!playerInRange || !booleanAccessor.GetBoolFromThis("wolfDone")) return;

        if (!weaponHandled) HandleWeaponTask();
        if (weaponClone == null) return;

        StatusTask();
    }

    private void HandleWeaponTask()
    {
        var itemClone = GetPlayerFucile();
        if (itemClone == null) { fucile.GetComponent<ItemPickup>().enabled = true; return; }
        weaponHandled = true;
        weaponClone = itemClone as Weapon;
        if (fucile.activeSelf)
        {
            player.GetComponent<ItemDetector>().RemoveItemDetection(weaponClone.prefab);
            fucile.SetActive(false);
            weaponClone.prefab.GetComponent<ItemPickup>().enabled = false;
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    private void StatusTask()
    {
        if (PlayerPrefs.GetInt("nTargetHit") < maxTargetHit && weaponClone.prefab.GetComponent<WeaponAmmo>().extraAmmo == 0 &&
            booleanAccessor.GetBoolFromThis("cocaCola")) // Ha perso
        {
            conversationManager.StartConversation(conversations[2]); // Parte UomoBaitaLost
            booleanAccessor.SetBoolOnDialogueE("cocaColaDone");
            TaskFailed();
            TaskReset();
            return;
        }

        if (PlayerPrefs.GetInt("nTargetHit") == maxTargetHit) // Ha vinto
        {
            conversationManager.StartConversation(conversations[1]); // Parte UomoBaitaWin
            booleanAccessor.SetBoolOnDialogueE("cocaColaDone");
            TaskReset();
            // Distruggere il playerprefs se non serve più nel gioco
            // OPPURE qualcosa per le statistiche di scena ? (es. "Stefano ha sparato 3 bottiglie con 3 colpi")
        }
    }

    public Item GetPlayerFucile() => InventoryManager.instance.items.Find(weapon => weapon.nameItem == "Fucile da caccia");

    public void VisibleAreaPickup() => areaTask.SetActive(true); // Invocato dal DE di UomoBaita2

    private void ResetPlayerProgress() { PlayerPrefs.SetInt("nTargetHit", 0); PlayerPrefs.Save(); }

    private void TaskFailed()
    {
        conversationManager.SetBool("cocaCola", false);
        booleanAccessor.ResetBoolValue("cocaCola");
        booleanAccessor.ResetBoolValue("cocaColaDone");
        fucile.GetComponent<ItemPickup>().enabled = true;
        ResetPlayerProgress();
    }

    private void TaskReset()
    {
        // Distruggere dalla scena l'arma clonata ?
        fucile.SetActive(true);
        GetComponent<BoxCollider>().isTrigger = true;
        InventoryManager.instance.Remove(weaponClone, true);
    }

    private void PlayerTrigger(Collider other, bool _playerInRange)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = _playerInRange;
        }
    }

    public void PlayerCollision()
    {
        if (isConversationActive) return;
        conversationManager.StartConversation(conversations[0]); // Parte UomoBaitaAlert
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerTrigger(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        PlayerTrigger(other, false);
    }
}

// Dopo aver concluso la missione del lupo:

// Vincoliamo il giocatore a seguire il tutorial:

// - L'uomo Baita dice a Stefano di raccogliere il fucile
// - Appena lo fa, compaiono 4 muri invisibili, tipo area trigger, se stefano Collide con essi, viene avviato un dialogo dell'uomo baita che dice "Non barare ragazzo, resta vicino al tronco".

// - Nel fucile inseriamo solo 10 proiettili, nessun limite di tempo, lui deve apprendere la meccanica del combattimento a distanza.

// - Il tutorial termina quando le 3 bottiglie sono state sparate OPPURE quando ha finito i proiettili del fucile.

// - Quando il tutorial termina, i muri invisibili vengono disattivati, l'arma si posiziona sul tronco, senza aura e senza pick-up, per far comprendere che non è interagibile, quindi il tutorial avviene solo una volta.
// Tanto se ha sparato 10 proiettili bene o male ha capito come si spara.

// - Ricompensa: Se spara 3 bottiglie e restano 7 proiettili significa che ha sparato con successo le 3 bottiglie con i primi 3 colpi, in tal caso gli conferiamo una ricompensa, altrimenti niente.
