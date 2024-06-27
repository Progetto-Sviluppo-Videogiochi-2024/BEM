using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationGeneral : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Appena comincia il gioco, parte la conversazione
        StartDialogue();
        
    }

    // Avvia la conversazione con il giocatore
    private void StartDialogue()
    {
        ConversationManager.Instance.StartConversation(dialogue);
    }

    private void StopConversation()
    {
        ConversationManager.Instance.EndConversation();
    }
}
