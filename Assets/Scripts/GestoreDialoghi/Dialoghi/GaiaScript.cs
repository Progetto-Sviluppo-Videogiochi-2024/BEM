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
            if (!booleanAccessor.GetBoolFromThis("fioriRaccolti"))
            {
                // Dialogo iniziale
                convManager.StartConversation(conversations[0]);
                convManager.SetBool("fioriRaccolti", booleanAccessor.GetBoolFromThis("fioriRaccolti"));
                convManager.SetBool("fiori", booleanAccessor.GetBoolFromThis("fiori"));

            }
            else if (booleanAccessor.GetBoolFromThis("fiori") && booleanAccessor.GetBoolFromThis("fioriRaccolti"))
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