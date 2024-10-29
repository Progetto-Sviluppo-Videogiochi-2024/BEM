using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class JackobScript : MonoBehaviour
{
    public NPCConversation dialogo;
    public NPCConversation dialogo2;

    public static BooleanAccessor istance;

    private bool isInRange;

    private void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (BooleanAccessor.istance != null)
            {
                if (BooleanAccessor.istance.GetBoolFromThis("cartello") == false)
                {
                    // Dialogo iniziale
                    ConversationManager.Instance.StartConversation(dialogo);
                    ConversationManager.Instance.SetBool("cartello", BooleanAccessor.istance.GetBoolFromThis("cartello"));
                }
                else
                {
                    // Dialogo successivo
                    ConversationManager.Instance.StartConversation(dialogo2);
                    ConversationManager.Instance.SetBool("tolto", BooleanAccessor.istance.GetBoolFromThis("tolto"));
                }
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }
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
            ConversationManager.Instance.EndConversation();
        }
    }
}
