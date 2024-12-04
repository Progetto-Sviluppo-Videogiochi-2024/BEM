using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class WallCollision : MonoBehaviour
{
    public NPCConversation conversations; // Conversazioni con l'uomo baita per il tutorial del fucile
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ha colpito il muro");
            ConversationManager.Instance.StartConversation(conversations);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player ha lasciato il muro");
        }
    }
}
