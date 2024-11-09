using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class TutorialScript : MonoBehaviour
{
    public NPCConversation dialogo;
    public NPCConversation dialogo2;
    public NPCConversation dialogo3;
    public static BooleanAccessor istance;
    //public bool isConversationActive = false;  // Stato della conversazione

    public void StartTutorial()
    {
        if (BooleanAccessor.istance != null)
        {
            if (BooleanAccessor.istance.GetBoolFromThis("premereE") == false)
            {
                // Dialogo iniziale
                ConversationManager.Instance.StartConversation(dialogo);
                ConversationManager.Instance.SetBool("premereE", BooleanAccessor.istance.GetBoolFromThis("cartello"));
            }
            else if (BooleanAccessor.istance.GetBoolFromThis("wasd") == false)
            {
                // Dialogo successivo
                ConversationManager.Instance.StartConversation(dialogo2);
                ConversationManager.Instance.SetBool("wasd", BooleanAccessor.istance.GetBoolFromThis("tolto"));
            }
            else
            {
                // Dialogo successivo
                ConversationManager.Instance.StartConversation(dialogo3);
                ConversationManager.Instance.SetBool("x", BooleanAccessor.istance.GetBoolFromThis("tolto"));
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance non è stato inizializzato.");
        }
    }
}
