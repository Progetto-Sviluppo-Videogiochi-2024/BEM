using DialogueEditor;
using UnityEngine;
using UnityEngine.UI;

public class Door : NPCDialogueBase
{
    [Header("Settings")]
    #region Settings
    public bool canOpen = false; // Indica se la porta può essere aperta
    #endregion

    [Header("References")]
    #region References
    public RadioManager radio; // Riferimento alla radio
    public GestoreScena gestoreScena; // Riferimento al GestoreScena
    public GameObject confirmNextUI; // Riferimento al pannello di conferma per andare alla scena successiva
    private GameObject yesButton; // Riferimento al pulsante "Sì" del pannello di conferma
    private GameObject noButton; // Riferimento al pulsante "No" del pannello di conferma
    #endregion

    void Start()
    {
        var panel = confirmNextUI.transform.GetChild(0);
        yesButton = panel.Find("YesButton").gameObject;
        noButton = panel.Find("NoButton").gameObject;
    }

    protected override void StartDialogue()
    {
        if (!canOpen || radio.isOn) // Se la radio è accesa e non può aprire la porta (oggetti non raccolti)
        {
            ConversationManager.Instance.StartConversation(conversations[0]);
        }
        else if (canOpen && !radio.isOn) // Se la radio è spenta e può aprire la porta (oggetti raccolti)
        {
            ToggleCursor(true); // Poi si disattiva cliccando su End
            confirmNextUI.SetActive(true);
            yesButton.GetComponent<Button>().onClick.AddListener(OnYesButtonClicked);
            noButton.GetComponent<Button>().onClick.AddListener(OnNoButtonClicked);
        }
    }

    private void OnYesButtonClicked()
    {
        ToggleCursor(false);
        gestoreScena.GoToTransitionScene();
        ConversationManager.Instance.hasClickedEnd = true;
    }

    private void OnNoButtonClicked()
    {
        ToggleCursor(false);
        confirmNextUI.SetActive(false);
        ConversationManager.Instance.hasClickedEnd = true;
    }

    private void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
