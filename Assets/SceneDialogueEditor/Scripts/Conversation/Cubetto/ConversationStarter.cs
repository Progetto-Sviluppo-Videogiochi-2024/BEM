using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class ConversationStarter : MonoBehaviour
{
    [SerializeField] private NPCConversation dialogue;
    private bool isInRange;
    
    // Start is called before the first frame update
    void Start()
    {
        isInRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Se il giocatore è nell'area di trigger e preme il tasto F, inizia la conversazione
        if (isInRange && Input.GetKeyDown(KeyCode.F))
        {
            StartDialogue();
        }   
    }

    // Inizia la conversazione con il giocatore se è nell'area di trigger 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    // Termina la conversazione con il giocatore se esce dall'area di trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            StopConversation();
        }
    }

    // Continua la conversazione con il giocatore se è nell'area di trigger
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
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
