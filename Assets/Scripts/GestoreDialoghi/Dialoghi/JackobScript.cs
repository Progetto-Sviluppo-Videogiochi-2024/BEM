using UnityEngine;
using DialogueEditor;

public class JackobScript : NPCDialogueBase
{
    public static BooleanAccessor istance;

    protected override void StartDialogue()
    {
        if (BooleanAccessor.istance != null)
        {
            if (BooleanAccessor.istance.GetBoolFromThis("cartello") == false)
            {
                // Dialogo iniziale
                StartConversation(dialogo);
                ConversationManager.Instance.SetBool("cartello", BooleanAccessor.istance.GetBoolFromThis("cartello"));
            }
            else
            {
                // Dialogo successivo
                StartConversation(dialogo2);
                ConversationManager.Instance.SetBool("tolto", BooleanAccessor.istance.GetBoolFromThis("tolto"));
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance non è stato inizializzato.");
        }
    }
}
