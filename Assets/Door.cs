using DialogueEditor;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public bool canOpen = false; // Indica se la porta può essere aperta
    private bool canInteract = false; // Indica se il player può interagire con la porta
    #endregion

    [Header("References")]
    #region References
    public NPCConversation DoorBlockedDialogue; // Conversazione se non può uscire dalla porta
    public Radio radio; // Riferimento alla radio
    public GestoreScena gestoreScena; // Riferimento al GestoreScena
    #endregion

    void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.Space))
        {
            if (!canOpen && radio.isOn) // Se la radio è accesa e non può aprire la porta (oggetti non raccolti)
            {
                var convManager = ConversationManager.Instance;
                convManager.StartConversation(DoorBlockedDialogue);
            }
            else if (canOpen && !radio.isOn) // Se la radio è spenta e può aprire la porta (oggetti raccolti)
            {
                // Chiedere al giocatore se vuole andare alla scena successiva
                gestoreScena.GoToTransitionScene();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) canInteract = false;
    }
}
