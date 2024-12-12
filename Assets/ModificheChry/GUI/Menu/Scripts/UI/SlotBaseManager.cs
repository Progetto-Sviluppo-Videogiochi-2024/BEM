using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SlotBaseManager : MonoBehaviour
{
    [Header("UI Slot")]
    #region UI Slot
    [SerializeField] protected Transform content; // Content che contiene gli slot
    [SerializeField] protected Transform confirmSlot; // Pannello di conferma del caricamento/salvataggio
    [SerializeField] protected Button yesButton; // Bottone di conferma del confirmSlot
    [SerializeField] protected Button noButton; // Bottone di annullamento del confirmSlot
    [SerializeField] protected GameObject infoSceneSlot; // Pannello con le info della scena da caricare/salvare
    #endregion

    [Header("References")]
    #region References
    protected SaveLoadSystem saveLoadSystem; // Riferimento al SaveLoadSystem
    [SerializeField] protected GestoreScena gestoreScena; // Riferimento al GestoreScena
    #endregion

    [Header("Slot Settings")]
    #region Slot Settings
    protected string currentSlotOpened = null; // Nome dello slot aperto da cui confermare il caricamento/salvataggio
    #endregion

    protected void Start() => infoSceneSlot.SetActive(false);

    protected void OnEnable() => ListSlotUI();

    protected abstract void LoadSlotUI(Transform slot, string savedSlotName);

    public void ListSlotUI()
    {
        if (saveLoadSystem == null) saveLoadSystem = SaveLoadSystem.Instance;

        var savedSlots = saveLoadSystem.dataService.ListSaves();
        print($"Numero di slot salvati: {savedSlots.Count()}");

        var slotList = content.Cast<Transform>().ToList();

        var savedSlotQueue = new Queue<string>(savedSlots);
        print($"Slot Queue: {string.Join(", ", savedSlotQueue.ToArray())}");

        for (int i = 0; i < slotList.Count; i++)
        {
            var currentUISlot = slotList[i];

            // Se ci sono ancora salvataggi disponibili
            if (savedSlotQueue.Count > 0)
            {
                var savedSlotName = savedSlotQueue.Peek();
                print($"Slot salvato: {savedSlotName} per lo slot {currentUISlot.name}");

                if (saveLoadSystem.dataService.SearchSlotFileByUI(savedSlotName, currentUISlot.name))
                {
                    print($"Slot {currentUISlot.name} - {savedSlotName}");
                    ToggleSlotUI(currentUISlot, false);
                    LoadSlotUI(currentUISlot, savedSlotName);
                    savedSlotQueue.Dequeue();
                }
                else // Slot non corrispondente
                {
                    ToggleSlotUI(currentUISlot, true);
                    ConfigureEmptySlot(currentUISlot);
                }
            }
            else // Non ci sono più salvataggi, configura lo slot come vuoto
            {
                ToggleSlotUI(currentUISlot, true);
                ConfigureEmptySlot(currentUISlot);
            }
        }
    }

    protected void DeleteSlotUI(Transform slot) // Invocata dal bottone "-" figlio di ogni slot del LG
    {
        // Quando listo gli slot già controllo se lo slot è presente
        saveLoadSystem.DeleteGame(slot.name);
        ToggleSlotUI(slot, true);
    }

    protected void ConfigureSlotButton(Button button, UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }

    protected void ConfigureEmptySlot(Transform slot)
    {
        var actionButton = slot.GetComponent<Button>();
        ConfigureSlotButton(actionButton, () => OpenConfirmPanel(slot.name));
    }

    protected void ToggleSlotUI(Transform slot, bool active)
    {
        slot.GetChild(0).gameObject.SetActive(active); // Per "Empty"
        slot.GetChild(1).gameObject.SetActive(!active); // Per "Saved"
    }

    protected void SetMouseHoverSlot(Transform slot)
    {
        // Per evitare trigger multipli
        var eventTrigger = slot.GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = slot.gameObject.AddComponent<EventTrigger>();
        else eventTrigger.triggers.Clear();

        // Configura l'evento OnPointerEnter
        var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((eventData) => OnSlotEnter(infoSceneSlot.transform));
        eventTrigger.triggers.Add(pointerEnterEntry);

        // Configura l'evento OnPointerExit
        var pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((eventData) => OnSlotExit(infoSceneSlot.transform));
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    public void OnSlotEnter(Transform infoSceneSlot)
    {
        infoSceneSlot.gameObject.SetActive(true);
        // Aggiungere IMG (img, una per ogni scena) e Descrizione (testo, uno per ogni scena) per infoSceneSlot
    }

    public void OnSlotExit(Transform infoSceneSlot) => infoSceneSlot.gameObject.SetActive(false);

    protected void OpenConfirmPanel(string slotName)
    {
        currentSlotOpened = slotName;
        confirmSlot.gameObject.SetActive(true);
        confirmSlot.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = GetConfirmMessage();

        ConfigureSlotButton(yesButton, OnYesConfirmSlot);
        ConfigureSlotButton(noButton, OnNoConfirmSlot);
    }

    public void OnYesConfirmSlot() { confirmSlot.gameObject.SetActive(false); OnYesActionSlot(); }

    public void OnNoConfirmSlot() { confirmSlot.gameObject.SetActive(false); currentSlotOpened = null; }

    public abstract void OnYesActionSlot();

    protected abstract string GetConfirmMessage();
}
