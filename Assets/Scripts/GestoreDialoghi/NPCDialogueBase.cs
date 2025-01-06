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
    protected static bool clickEndHandled = true; // Flag per gestire la fine della conversazione una sola volta
    protected bool isConversationActive = false;  // Stato della conversazione
    protected bool isInRange = false;  // Se il giocatore è nel raggio
    #endregion

    [Header("References")]
    #region References
    public GameObject player; // Riferimento al giocatore
    #endregion

    protected abstract void StartDialogue(); // Metodo astratto per la logica personalizzata (override nelle classi derivate)
    protected abstract void EndDialogue(); // Metodo astratto per la logica personalizzata (override nelle classi derivate)

    protected virtual void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            isConversationActive = false;
            clickEndHandled = true;

            // print("NPCDialogueBase.Update 1.if: " + gameObject.name);
            GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
            player.GetComponent<MovementStateManager>().enabled = true;
            EndDialogue();
        }

        if (isInRange && !isConversationActive && Input.GetKeyDown(KeyCode.Space))
        {
            ConversationManager.Instance.hasClickedEnd = false;
            clickEndHandled = false;
            // print("NPCDialogueBase.Update 2.if: " + gameObject.name);

            StartDialogue(); // Avvia il dialogo (metodo astratto che deve invocare StartConversation)
            player.GetComponent<MovementStateManager>().enabled = false;
        }
    }

    protected void StartConversation(NPCConversation dialog)
    {
        GestoreScena.ChangeCursorActiveStatus(true, "NPCDialogueBase.StartConversation: " + gameObject.transform.parent.name);

        player.GetComponent<Animator>().SetFloat("hInput", 0);
        player.GetComponent<Animator>().SetFloat("vInput", 0);

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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = true; // Quando il giocatore entra nell'area
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isInRange = false; // Quando il giocatore esce dall'area
    }
}
