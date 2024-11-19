using UnityEngine;
using DialogueEditor;

public abstract class NPCDialogueBase : MonoBehaviour
{
    public NPCConversation[] conversations; // Array di dialoghi
    public bool isConversationActive = false;  // Stato della conversazione
    private bool isInRange = false;  // Se il giocatore è nel raggio
    public GameObject player; // Riferimento al giocatore

    protected abstract void StartDialogue(); // Metodo astratto per la logica personalizzata (da implementare nelle classi derivate)

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
            StartDialogue(); // Chiama la logica specifica per iniziare il dialogo
            player.GetComponent<MovementStateManager>().enabled = false;
        }
    }

    protected void StartConversation(NPCConversation dialog)
    {
        isConversationActive = true;
        ConversationManager.Instance.StartConversation(dialog);
    }

    public void SetMousePointerState(bool enable)
    {
        Cursor.visible = enable; // Imposta la visibilità del cursore
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked; // Sblocca o blocca il cursore
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
        if (other.CompareTag("Player"))
        {
            isInRange = true; // Imposta isInRange a true quando il giocatore entra
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false; // Imposta isInRange a false quando il giocatore esce dall'area
        }
    }
}
