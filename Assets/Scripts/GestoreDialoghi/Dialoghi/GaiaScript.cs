using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class GaiaScript : MonoBehaviour
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
                if (BooleanAccessor.istance.GetBoolFromThis("fiore") == false)
                {
                    //Dialogo iniziale
                    ConversationManager.Instance.StartConversation(dialogo);
                    ConversationManager.Instance.SetBool("fiore", BooleanAccessor.istance.GetBoolFromThis("cartello"));
                }
                else
                {
                    //Dialogo successivo
                    ConversationManager.Instance.StartConversation(dialogo2);
                    ConversationManager.Instance.SetBool("soluzione", BooleanAccessor.istance.GetBoolFromThis("tolto"));
                }
            }
            else
            {
                Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
            }

        }
    }

}

