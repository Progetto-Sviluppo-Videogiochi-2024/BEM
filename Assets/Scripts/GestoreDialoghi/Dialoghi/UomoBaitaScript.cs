using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class UomoBaitaScript : MonoBehaviour
{
    public NPCConversation dialogo;
    public NPCConversation dialogo2;
    public NPCConversation dialogo3;

    public static BooleanAccessor istance;

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (BooleanAccessor.istance != null)
            {
                if (BooleanAccessor.istance.GetBoolFromThis("acqua") == false)
                {
                    //Dialogo iniziale
                    ConversationManager.Instance.StartConversation(dialogo);
                    ConversationManager.Instance.SetBool("acqua", BooleanAccessor.istance.GetBoolFromThis("acqua"));
                }
                else
                {
                    //Dialogo successivo
                    ConversationManager.Instance.StartConversation(dialogo2);
                    ConversationManager.Instance.SetBool("acquaDone", BooleanAccessor.istance.GetBoolFromThis("acquaDone"));
                }
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }
        }
    }

}

