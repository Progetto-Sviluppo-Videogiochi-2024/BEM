using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class AngelicaScript : MonoBehaviour
{
    public NPCConversation dialogo;
    private bool isInRange;
    private bool isConversationActive = false;

    private void OnEnable()
    {
        // Collega l'evento quando lo script è abilitato
        ConversationManager.OnConversationEnded += HandleConversationEnded;
    }

    private void OnDisable()
    {
        // Scollega l'evento quando lo script è disabilitato
        ConversationManager.OnConversationEnded -= HandleConversationEnded;
    }

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            StartConversation(); // Inizia la conversazione solo se non è già attiva
        }
    }

    private void StartConversation()
    {
        if (!isConversationActive)
        {
            isConversationActive = true;
            ConversationManager.Instance.StartConversation(dialogo);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            EndConversation(); 
        }
    }

    private void EndConversation()
    {
        if (isConversationActive)
        {
            ConversationManager.Instance.EndConversation();
            isConversationActive = false;
        }
    }

    private void HandleConversationEnded()
    {
        // Reimposta la variabile quando la conversazione termina
        isConversationActive = false;
    }
}
