using UnityEngine;
using DialogueEditor;

public class GaiaScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        var booleanAccessor = BooleanAccessor.istance;
        if (booleanAccessor != null)
        {
            var convManager = ConversationManager.Instance;
            if (booleanAccessor.GetBoolFromThis("fiore") == false)
            {
                // Dialogo iniziale
                convManager.StartConversation(conversations[0]);
                convManager.SetBool("fiore", booleanAccessor.GetBoolFromThis("fiore"));
            }
            else
            {
                // Dialogo successivo
                convManager.StartConversation(conversations[1]);
                convManager.SetBool("soluzione", booleanAccessor.GetBoolFromThis("soluzione"));
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance non è stato inizializzato.");
        }
    }
}