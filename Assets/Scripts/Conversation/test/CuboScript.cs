using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class CuboScript : MonoBehaviour
{
    public NPCConversation dialogo;
    public NPCConversation dialogo2;

    public static BooleanAccessor istance;

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (BooleanAccessor.istance != null)
            {
                if (BooleanAccessor.istance.GetBoolFromThis("assegnata") == false)
                {
                    //Dialogo iniziale
                    ConversationManager.Instance.StartConversation(dialogo);
                    ConversationManager.Instance.SetBool("assegnta", BooleanAccessor.istance.GetBoolFromThis("assegnata"));
                }
                else
                {
                    //Dialogo successivo
                    ConversationManager.Instance.StartConversation(dialogo2);
                    ConversationManager.Instance.SetBool("detto", BooleanAccessor.istance.GetBoolFromThis("detto"));
                }
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }

        }
    }

}

