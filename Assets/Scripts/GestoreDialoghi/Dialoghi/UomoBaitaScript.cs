using UnityEngine;
using DialogueEditor;

public class UomoBaitaScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor != null)
        {
            var convManager = ConversationManager.Instance;
            if (!boolAccessor.GetBoolFromThis("wolf"))
            {
                // Dialogo iniziale
                StartConversation(conversations[0]);
                convManager.SetBool("wolf", boolAccessor.GetBoolFromThis("wolf"));
            }
            else
            {
                // Dialogo successivo
                StartConversation(conversations[1]);
                convManager.SetBool("wolfDone", boolAccessor.GetBoolFromThis("wolfDone"));
            }
        }
        else
        {
            Debug.LogError("BoolAccessor non è stato inizializzato.");
        }
    }
}