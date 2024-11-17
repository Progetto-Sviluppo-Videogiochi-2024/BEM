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
    public Radio radio; // Riferimento alla radio
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
            // Chiedere al giocatore se vuole andare alla scena successiva
            confirmNextUI.SetActive(true);
            yesButton.GetComponent<Button>().onClick.AddListener(OnYesButtonClicked);
            noButton.GetComponent<Button>().onClick.AddListener(OnNoButtonClicked);
        }
    }

    private void OnYesButtonClicked()
    {
        gestoreScena.GoToTransitionScene();
    }

    private void OnNoButtonClicked()
    {
        confirmNextUI.SetActive(false);
    }
}
