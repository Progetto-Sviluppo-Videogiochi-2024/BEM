using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class EscursionistiScript : MonoBehaviour
{
    public NPCConversation dialogo;
    //public NPCConversation dialogo2;

    //public static BooleanAccessor istance;

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ConversationManager.Instance.StartConversation(dialogo);  
        }
    }

}

