using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class AngelicaScript : MonoBehaviour
{
    public NPCConversation dialogo;
    private bool isInRange;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            StartConversation(); // Inizia la conversazione quando il giocatore preme la barra spaziatrice
        }
    }

    private void StartConversation()
    {
        ConversationManager.Instance.StartConversation(dialogo);
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
            ConversationManager.Instance.EndConversation();
        }
    }
}
