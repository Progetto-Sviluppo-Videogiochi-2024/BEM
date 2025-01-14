using DialogueEditor;
using UnityEngine;
using UnityEngine.UI;

public class DoorDialogue : NPCDialogueBase
{
    [Header("Settings")]
    #region Settings
    public bool canOpen = false; // Indica se la porta può essere aperta
    #endregion

    [Header("References")]
    #region References
    public GestoreScena gestoreScena; // Riferimento al GestoreScena
    public RadioManager radio; // Riferimento alla radio
    public GameObject confirmNextUI; // Riferimento al pannello di conferma per andare alla scena successiva
    private GameObject yesButton; // Riferimento al pulsante "Sì" del pannello di conferma
    private GameObject noButton; // Riferimento al pulsante "No" del pannello di conferma
    #endregion

    protected override void Start()
    {
        base.Start();
        var panel = confirmNextUI.transform.GetChild(0);
        yesButton = panel.Find("YesButton").gameObject;
        noButton = panel.Find("NoButton").gameObject;
    }

    protected override void StartDialogue()
    {
        if (!canOpen || radio.isOn) // Se la radio è accesa e non può aprire la porta (oggetti non raccolti)
        {
            GestoreScena.ChangeCursorActiveStatus(true, "door.startDialogue1"); // Poi si disattiva quando hasClickedEnd = true
            ConversationManager.Instance.StartConversation(conversations[0]);
        }
        else if (canOpen && !radio.isOn) // Se la radio è spenta e può aprire la porta (oggetti raccolti)
        {
            GestoreScena.ChangeCursorActiveStatus(true, "door.startDialogue2"); // Poi si disattiva quando hasClickedEnd = true
            confirmNextUI.SetActive(true);
            yesButton.GetComponent<Button>().onClick.AddListener(OnYesButtonClicked);
            noButton.GetComponent<Button>().onClick.AddListener(OnNoButtonClicked);
        }
    }

    protected override void EndDialogue() { }

    private void OnYesButtonClicked()
    {
        GestoreScena.ChangeCursorActiveStatus(false, "door.yesButton");
        gestoreScena.GoToTransitionScene();
        ConversationManager.Instance.hasClickedEnd = true;
    }

    private void OnNoButtonClicked()
    {
        GestoreScena.ChangeCursorActiveStatus(false, "door.noButton");
        confirmNextUI.SetActive(false);
        ConversationManager.Instance.hasClickedEnd = true;
    }
}
