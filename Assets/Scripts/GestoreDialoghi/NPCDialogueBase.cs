using UnityEngine;
using DialogueEditor;

public abstract class NPCDialogueBase : MonoBehaviour
{
    [Header("Conversations")]
    #region Conversations
    public NPCConversation[] conversations; // Array di dialoghi
    #endregion

    [Header("Settings")]
    #region Settings
    public bool isConversationActive = false;  // Stato della conversazione
    private bool isInRange = false;  // Se il giocatore è nel raggio
    #endregion

    [Header("References")]
    #region References
    public GameObject player; // Riferimento al giocatore
    #endregion

    protected abstract void StartDialogue(); // Metodo astratto per la logica personalizzata (override nelle classi derivate)

    private void Update()
    {
        if (ConversationManager.Instance.hasClickedEnd)
        {
            isConversationActive = false;
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (isInRange && !isConversationActive && Input.GetKeyDown(KeyCode.Space))
        {
            ConversationManager.Instance.hasClickedEnd = false;
            StartDialogue(); // Avvia il dialogo (metodo astratto che deve invocare StartConversation)
            player.GetComponent<MovementStateManager>().enabled = false;
        }
    }

    protected void StartConversation(NPCConversation dialog)
    {
        // Blocco il cursore in UIConversationButton.DoClickBehaviour(): in end
        Cursor.visible = true; // Imposta la visibilità del cursore
        Cursor.lockState = CursorLockMode.None; // Sblocca il cursore

        isConversationActive = true;
        ConversationManager.Instance.StartConversation(dialog);
    }

    private void HandleConversationEnded()
    {
        isConversationActive = false; // Reimposta lo stato della conversazione
    }

    private void OnEnable()
    {
        ConversationManager.OnConversationEnded += HandleConversationEnded; // Collega l'evento alla fine della conversazione
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationEnded -= HandleConversationEnded; // Scollega l'evento
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = true; // Quando il giocatore entra nell'area
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = false; // Quando il giocatore esce dall'area
    }
}
