using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public abstract class NPCDialogueBase : MonoBehaviour
{
    public NPCConversation dialogo;
    public NPCConversation dialogo2;
    public NPCConversation dialogo3;
    public bool isConversationActive = false;  // Stato della conversazione
    private bool isInRange = false;  // Se il giocatore è nel raggio

    // Metodo astratto per la logica personalizzata (da implementare nelle classi derivate)
    protected abstract void StartDialogue();

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space) && !isConversationActive)
        {
            StartDialogue(); // Chiama la logica specifica per iniziare il dialogo
        }
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
            EndConversation(); // Termina la conversazione se il giocatore esce
        }
    }

    protected void EndConversation()
    {
        if (isConversationActive)
        {
            ConversationManager.Instance.EndConversation();
            isConversationActive = false; // Imposta isConversationActive a false quando la conversazione finisce
        }
    }

    protected void StartConversation(NPCConversation dialog)
    {
        isConversationActive = true;
        ConversationManager.Instance.StartConversation(dialog);
    }

    private void OnEnable()
    {
        ConversationManager.OnConversationEnded += HandleConversationEnded; // Collega l'evento alla fine della conversazione
    }

    private void OnDisable()
    {
        ConversationManager.OnConversationEnded -= HandleConversationEnded; // Scollega l'evento
    }

    private void HandleConversationEnded()
    {
        isConversationActive = false; // Reimposta lo stato della conversazione
    }
}
