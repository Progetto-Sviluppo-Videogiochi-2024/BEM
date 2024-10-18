using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;


public class SferaScript : MonoBehaviour
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
                if (BooleanAccessor.istance.GetBoolFromThis("cuboChat") == false)
                {
                    //Dialogo iniziale
                    ConversationManager.Instance.StartConversation(dialogo);
                    ConversationManager.Instance.SetBool("cuboChat", BooleanAccessor.istance.GetBoolFromThis("cuboChat"));
                }
                else
                {
                    //Dialogo successivo
                    ConversationManager.Instance.StartConversation(dialogo2);
                }
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }

        }
    }

}

