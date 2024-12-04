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
    public GameObject paretiAreaTask; // Riferimento alle pareti dell'area task
    public Transform player; // Riferimento al player
    public NPCConversation[] conversations; // Conversazioni con l'uomo baita per il tutorial del fucile
    [HideInInspector] public GameObject areaTask; // Riferimento a dove il player può raccogliere il fucile e iniziare il tutorial
    #endregion

    [Header("Settings")]
    #region Settings
    private bool weaponHandled = false; // Indica se l'arma è stata gestita
    [HideInInspector] public bool playerInRange = false; // Indica se è vicino al tronco
    private const int maxTargetHit = 3; // Numero di bottiglie da colpire per completare il tutorial
    #endregion

    void Start()
    {
        // P.S.: lascio l'aura attiva per far capire che è interagibile solo dopo la quest del lupo
        paretiAreaTask.SetActive(false);
        conversationManager = ConversationManager.Instance;
        booleanAccessor = BooleanAccessor.istance;
        fucile.GetComponent<ItemPickup>().enabled = false;
        ResetPlayerProgress();
    }

    void Update()
    {
        // P.S.: controlli preliminari per poter attivare/disattivare il tutorial del fucile
        if (booleanAccessor.GetBoolFromThis("cocaColaDone")) { enabled = false; return; }
        if (!playerInRange || !booleanAccessor.GetBoolFromThis("wolfDone")) return;

        if (!weaponHandled && booleanAccessor.GetBoolFromThis("cocaCola")) HandleWeaponTask();
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
            paretiAreaTask.SetActive(true);
            fucile.SetActive(false);
            weaponClone.prefab.GetComponent<ItemPickup>().enabled = false;
        }
    }

    private void StatusTask()
    {
        if (PlayerPrefs.GetInt("nTargetHit") < maxTargetHit && weaponClone.prefab.GetComponent<WeaponAmmo>().extraAmmo == 0 &&
            booleanAccessor.GetBoolFromThis("cocaCola")) // Ha perso
        {
            conversationManager.StartConversation(conversations[2]); // Parte UomoBaitaLost
            booleanAccessor.SetBoolOnDialogueE("cocaColaDone");
            TaskReset();
            return;
        }

        if (PlayerPrefs.GetInt("nTargetHit") == maxTargetHit) // Ha vinto
        {
            conversationManager.StartConversation(conversations[1]); // Parte UomoBaitaWin
            booleanAccessor.SetBoolOnDialogueE("cocaColaDone");
            TaskReset();
            // Distruggere il playerprefs se non serve più nel gioco || qualcosa per le statistiche di scena ?
        }
    }

    public Item GetPlayerFucile() => InventoryManager.instance.items.Find(weapon => weapon.nameItem == "Fucile da caccia");

    private void ResetPlayerProgress() { PlayerPrefs.SetInt("nTargetHit", 0); PlayerPrefs.Save(); }

    private void TaskReset()
    {
        InventoryManager.instance.Remove(weaponClone, true);
        fucile.SetActive(true);
        paretiAreaTask.SetActive(false);
        Destroy(weaponClone);
    }

    private void PlayerTrigger(Collider other, bool _playerInRange)
    {
        if (other.CompareTag("Player")) playerInRange = _playerInRange;
    }

    void OnTriggerEnter(Collider other) => PlayerTrigger(other, true);

    void OnTriggerExit(Collider other) => PlayerTrigger(other, false);
}
