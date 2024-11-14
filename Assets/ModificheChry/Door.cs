using DialogueEditor;
using UnityEngine;

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
    #endregion

    protected override void StartDialogue()
    {
        if (!canOpen || radio.isOn) // Se la radio è accesa e non può aprire la porta (oggetti non raccolti)
        {
            ConversationManager.Instance.StartConversation(conversations[0]);
        }
        else if (canOpen && !radio.isOn) // Se la radio è spenta e può aprire la porta (oggetti raccolti)
        {
            // Chiedere al giocatore se vuole andare alla scena successiva
            gestoreScena.GoToTransitionScene();
        }
    }
}
