using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SlotBaseManager : MonoBehaviour
{
    [Header("UI Slot")]
    #region UI Slot
    [SerializeField] private GameObject loadSavePanel; // Pannello di caricamento/salvataggio
    [SerializeField] protected Transform content; // Content che contiene gli slot
    [SerializeField] protected Transform confirmSlot; // Pannello di conferma del caricamento/salvataggio
    [SerializeField] protected Button yesButton; // Bottone di conferma del confirmSlot
    [SerializeField] protected Button noButton; // Bottone di annullamento del confirmSlot
    [SerializeField] protected GameObject infoSceneSlot; // Pannello con le info della scena da caricare/salvare
    #endregion

    [Header("References")]
    #region References
    private GameObject player; // Riferimento al giocatore
    protected SaveLoadSystem saveLoadSystem; // Riferimento al SaveLoadSystem
    [SerializeField] protected GestoreScena gestoreScena; // Riferimento al GestoreScena
    #endregion

    [Header("Slot Settings")]
    #region Slot Settings
    protected bool isClickedOnDelete = false; // Flag per il click sul bottone "-" di uno slot per eliminare il salvataggio
    protected string currentSlotOpened = null; // Nome dello slot aperto da cui confermare il caricamento/salvataggio
    #endregion

    protected void Awake()
    {
        loadSavePanel.SetActive(false);
        infoSceneSlot.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        saveLoadSystem = SaveLoadSystem.Instance;
    }

    protected void OnEnable() => ListSlotUI();

    public void ToggleLoadSaveUI(bool active)
    {
        loadSavePanel.SetActive(active);
        var currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != "MainMenu" && currentScene != "Transizione")
            GestoreScena.ChangeCursorActiveStatus(active, "SlotBaseManager.ToggleUI: " + loadSavePanel.name);
        if (player != null)
        {
            player.GetComponent<Animator>().SetFloat("hInput", 0);
            player.GetComponent<Animator>().SetFloat("vInput", 0);
            player.GetComponent<MovementStateManager>().enabled = !active;
            player.GetComponent<AimStateManager>().enabled = !active;
        }
        Time.timeScale = active ? 0 : 1; // 0 = pausa, 1 = gioco normale
        if (confirmSlot.gameObject.activeSelf) confirmSlot.gameObject.SetActive(false);
    }

    protected void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = saveLoadSystem.dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot nSlotSave"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        var deleteButton = slotSavedList.GetComponentInChildren<Button>();
        ConfigureSlotButton(deleteButton, () => { OpenConfirmDeletePanel(slot); }); // Per "Delete" al click
        SetMouseHoverSlot(deleteButton.transform, () => { isClickedOnDelete = true; }, () => { isClickedOnDelete = false; }); // Per "Delete" all'hover

        ConfigureSlotButton(slot.GetComponent<Button>(), () => OpenConfirmPanel(savedSlotName)); // Per "Load/Save" al click
        SetMouseHoverSlot(slot, () => { OnSlotEnter(infoSceneSlot.transform, gameData.currentSceneName); }, () => { OnSlotExit(infoSceneSlot.transform); }); // Per mostrare le info della scena all'hover
    }

    public abstract void ListSlotUI();

    protected void DeleteSlotUI(Transform slot) // Invocata dal bottone "Delete" di ogni slot salvato
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

    protected void SetMouseHoverSlot(Transform slot, Action enter, Action exit) // Per mostrare le info della scena o per eliminare uno slot
    {
        // Per evitare trigger multipli
        var eventTrigger = slot.GetComponent<EventTrigger>();
        if (eventTrigger == null) eventTrigger = slot.gameObject.AddComponent<EventTrigger>();
        else eventTrigger.triggers.Clear();

        // Configura l'evento OnPointerEnter
        var pointerEnterEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnterEntry.callback.AddListener((eventData) => enter());
        eventTrigger.triggers.Add(pointerEnterEntry);

        // Configura l'evento OnPointerExit
        var pointerExitEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExitEntry.callback.AddListener((eventData) => exit());
        eventTrigger.triggers.Add(pointerExitEntry);
    }

    public void OnSlotEnter(Transform infoSceneSlot, string nomeCapitolo)
    {
        Debug.Log("OnSlotEnter: " + nomeCapitolo);
        infoSceneSlot.gameObject.SetActive(true);
        var (img, description) = gestoreScena.GetSpriteAndDescriptionChapter(nomeCapitolo);
        infoSceneSlot.GetChild(0).GetComponent<Image>().sprite = img;
        infoSceneSlot.GetChild(1).GetComponent<TextMeshProUGUI>().text = description;        
    }

    public void OnSlotExit(Transform infoSceneSlot) => infoSceneSlot.gameObject.SetActive(false);

    protected void OpenConfirmDeletePanel(Transform slot) // Per cancellare uno slot specifico
    {
        confirmSlot.gameObject.SetActive(true);
        confirmSlot.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = GetConfirmMessage();

        ConfigureSlotButton(yesButton, () => OnYesDeleteSlot(slot));
        ConfigureSlotButton(noButton, OnNoConfirmSlot);
    }

    protected void OpenConfirmPanel(string slotName) // Per salvare e caricare uno slot specifico
    {
        currentSlotOpened = slotName;
        confirmSlot.gameObject.SetActive(true);
        confirmSlot.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = GetConfirmMessage();

        ConfigureSlotButton(yesButton, OnYesConfirmSlot);
        ConfigureSlotButton(noButton, OnNoConfirmSlot);
    }

    public void OnYesConfirmSlot() // Per salvare e caricare uno slot specifico
    {
        confirmSlot.gameObject.SetActive(false);
        OnYesActionSlot();
    }

    public abstract void OnYesActionSlot();

    protected abstract void OnYesDeleteSlot(Transform slot); // Per cancellare uno slot specifico

    public void OnNoConfirmSlot() { confirmSlot.gameObject.SetActive(false); currentSlotOpened = null; isClickedOnDelete = false; }

    protected abstract string GetConfirmMessage();
}
