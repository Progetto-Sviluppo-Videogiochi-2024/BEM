using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;

public class AngelicaScript : MonoBehaviour
{
    public NPCConversation dialogo;

    //public static BooleanAccessor istance;

    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ConversationManager.Instance.StartConversation(dialogo);  
        }
    }

}

