using System.Collections;
using System.Linq;
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
    WeaponAmmo ammo; // Riferimento alle munizioni del fucile
    private Weapon weapon; // Riferimento al fucile (weapon.prefab è l'arma clonata)
    public GameObject fucile; // Riferimento al fucile
    public GameObject paretiAreaTask; // Riferimento alle pareti dell'area task
    public Transform player; // Riferimento al player
    public NPCConversation[] conversations; // Conversazioni con l'uomo baita per il tutorial del fucile
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
        if (weapon == null) return;

        StatusTask();
    }

    public Item GetPlayerFucile() => InventoryManager.instance.items.Find(weapon => weapon.nameItem == "Fucile da caccia");

    private void ResetPlayerProgress()
    {
        PlayerPrefs.SetInt("hasBait", SaveLoadSystem.Instance.gameData.levelData.playerPrefs.Where(p => p.key == "nTargetHit").Select(p => p.value).FirstOrDefault());
        PlayerPrefs.Save();
    }

    private void HandleWeaponTask()
    {
        var weaponInInventory = GetPlayerFucile();
        if (weaponInInventory == null) { fucile.GetComponent<ItemPickup>().enabled = true; return; }
        weaponHandled = true;
        weapon = weaponInInventory as Weapon;
        if (fucile.activeSelf)
        {
            player.GetComponent<ItemDetector>().RemoveItemDetection(weapon.prefab);
            paretiAreaTask.SetActive(true);
            fucile.SetActive(false);
            weapon.prefab.GetComponent<ItemPickup>().enabled = false;
            ammo = weapon.prefab.GetComponent<WeaponAmmo>();
        }
    }

    private void StatusTask()
    {
        if (PlayerPrefs.GetInt("nTargetHit") <= maxTargetHit && ammo.extraAmmo + ammo.currentAmmo == 0) // Non ne ho colpite 3 e ho finito le munizioni
        {
            StartCoroutine(HandleTaskWithSFX(2)); // UomoBaitaLost
        }
        else if (PlayerPrefs.GetInt("nTargetHit") == maxTargetHit) // Ne ho colpite 3
        {
            StartCoroutine(HandleTaskWithSFX((ammo.extraAmmo + ammo.currentAmmo == weapon.ammo.maxAmmo - maxTargetHit) ? 1 : 2)); // Se 1 è UomoBaitaWin, se 2 UomoBaitaLost
        }
        // else Non ne ho colpite 3 e ho ancora munizioni => task in corso
    }

    private IEnumerator HandleTaskWithSFX(int indexConversation)
    {
        yield return new WaitForSeconds(1.25f); // Aspetta che il suono si completi (modifica il tempo in base alla durata dell'SFX)
        conversationManager.StartConversation(conversations[indexConversation]);
        TaskReset();
    }

    private void TaskReset()
    {
        var aimPlayer = player.GetComponent<AimStateManager>();
        aimPlayer.SwitchState(null);
        aimPlayer.crosshair.SetActive(false);
        aimPlayer.animator.SetBool("aiming", false);
        aimPlayer.currentFov = aimPlayer.idleFov;

        weapon.prefab.SetActive(false);
        booleanAccessor.SetBoolOnDialogueE("cocaColaDone");
        InventoryManager.instance.Remove(weapon, true);
        fucile.SetActive(true);
        paretiAreaTask.SetActive(false);
        fucile.GetComponent<ItemPickup>().enabled = false;
    }

    private void PlayerTrigger(Collider other, bool _playerInRange)
    {
        if (other.CompareTag("Player")) playerInRange = _playerInRange;
    }

    void OnTriggerEnter(Collider other) => PlayerTrigger(other, true);

    void OnTriggerExit(Collider other) => PlayerTrigger(other, false);
}
