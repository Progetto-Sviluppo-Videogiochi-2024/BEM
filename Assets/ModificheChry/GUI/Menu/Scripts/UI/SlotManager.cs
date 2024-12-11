using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotManager : MonoBehaviour
{
    [Header("UI Slot")]
    #region UI Slot
    [SerializeField] private Transform content; // Content che contiene gli slot
    [SerializeField] private Transform confirmLoadSlot; // Pannello di conferma del caricamento
    [SerializeField] private GameObject infoSceneLoadSlot; // Pannello con le info della scena da caricare
    #endregion

    [Header("References")]
    #region References
    IDataService dataService; // Riferimento al DataService per accedere ai dati di salvataggio
    [SerializeField] private GestoreScena gestoreScena; // Riferimento al GestoreScena
    #endregion

    [Header("Slot Settings")]
    #region Slot Settings
    private string slotOpened; // Nome dello slot aperto da cui confermare il caricamento
    #endregion

    void Start()
    {
        infoSceneLoadSlot.SetActive(false);
    }

    public void ListSlotUI() // Invocata da Continue del MM e chissà da chi altro
    {
        var savedSlots = dataService.ListSaves().OrderBy(name => name).ToList();
        var slotList = content.Cast<Transform>().ToList();

        for (int i = 0; i < slotList.Count; i++)
        {
            var uiSlot_i = slotList[i];
            if (i < savedSlots.Count)
            {
                var savedSlotName_i = savedSlots[i];

                // Verifica se il salvataggio corrisponde a questo slot (utile anche per capire se il salvataggio è esistente nel suo PC)
                if (dataService.SearchSlotFileByUI(savedSlotName_i, uiSlot_i.name))
                {
                    ToggleSlotUI(uiSlot_i, false);
                    LoadSlotUI(uiSlot_i, savedSlotName_i);
                }
                else ToggleSlotUI(uiSlot_i, true);
            }
            else ToggleSlotUI(uiSlot_i, true);
        }
    }

    private void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot i + 1"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        var deleteButton = slotSavedList.GetComponentInChildren<Button>();
        deleteButton.onClick.RemoveAllListeners(); // Rimuovi eventuali listener precedenti per evitare duplicati
        deleteButton.onClick.AddListener(() => DeleteSlotUI(slot)); // Aggiungi il listener

        var loadButton = slot.GetComponent<Button>();
        loadButton.onClick.RemoveAllListeners(); // Rimuovi eventuali listener precedenti per evitare duplicati
        loadButton.onClick.AddListener(() => { confirmLoadSlot.gameObject.SetActive(true); slotOpened = savedSlotName; });
        SetMouseHoverSlot(slot);
    }

    public void DeleteSlotUI(Transform slot) // Invocata dal bottone "-" figlio di ogni slot del LG
    {
        // Quando listo gli slot già controllo se il salvataggio è presente
        dataService.Delete(slot.name);
        ToggleSlotUI(slot, true);
    }

    void ToggleSlotUI(Transform slot, bool active)
    {
        slot.GetChild(0).gameObject.SetActive(active); // Per "Empty"
        slot.GetChild(1).gameObject.SetActive(!active); // Per "Saved"
    }

    void SetMouseHoverSlot(Transform slot)
    {
        var eventTrigger = slot.gameObject.AddComponent<EventTrigger>();

        // Configura l'evento OnPointerEnter
        var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((eventData) => OnSlotEnter(infoSceneLoadSlot.transform));
        eventTrigger.triggers.Add(pointerEnterEntry);

        // Configura l'evento OnPointerExit
        var pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((eventData) => OnSlotExit(infoSceneLoadSlot.transform));
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    private void OnSlotEnter(Transform infoSceneLoadSlot)
    {
        infoSceneLoadSlot.gameObject.SetActive(true);
        // Aggiungere IMG (img, una per ogni scena) e Descrizione (testo, uno per ogni scena) per infoSceneLoadSlot
    }

    private void OnSlotExit(Transform infoSceneLoadSlot) => infoSceneLoadSlot.gameObject.SetActive(false);

    public void OnYesConfirmLoadSlot() => SaveLoadSystem.Instance.LoadGame(slotOpened); // Carica la scena di gioco dallo slot selezionato
}
