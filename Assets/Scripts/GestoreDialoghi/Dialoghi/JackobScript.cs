using UnityEngine;
using DialogueEditor;

public class JackobScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor != null)
        {
            var convManager = ConversationManager.Instance;
            if (boolAccessor.GetBoolFromThis("cartello") == false)
            {
                // Dialogo iniziale
                StartConversation(conversations[0]);
                convManager.SetBool("cartello", boolAccessor.GetBoolFromThis("cartello"));
            }
            else
            {
                // Dialogo successivo
                StartConversation(conversations[1]);
                convManager.SetBool("cartelloDone", boolAccessor.GetBoolFromThis("cartelloDone"));
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance non è stato inizializzato.");
        }
    }
}
