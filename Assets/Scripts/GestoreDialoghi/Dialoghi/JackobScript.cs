using UnityEngine;
using DialogueEditor;

public class JackobScript : NPCDialogueBase
{
    private bool isFirstTime = true;

    protected override void StartDialogue()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor != null)
        {
            var convManager = ConversationManager.Instance;
            if (!boolAccessor.GetBoolFromThis("cartello")) // Se non ha parlato con Jacob
            {
                // Dialogo iniziale
                StartConversation(conversations[0]);
                convManager.SetBool("cartello", boolAccessor.GetBoolFromThis("cartello"));
            }
            else // Se ha parlato con Jacob
            {
                StartConversation(conversations[isFirstTime ? 1 : 2]);
                if (isFirstTime) // Se è la prima volta che gli parla
                {
                    convManager.SetBool("cartelloDone", boolAccessor.GetBoolFromThis("cartelloDone"));
                }
            }
        }
        else
        {
            Debug.LogError("BooleanAccessor.instance don't assigned to the script. Please assign it in the inspector.");
        }
    }

    protected override void EndDialogue() { }

    public void ResetFirstTime() => isFirstTime = false;
}
