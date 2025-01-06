using UnityEngine;
using DialogueEditor;

public class GaiaScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        var booleanAccessor = BooleanAccessor.istance; // Accesso al gestore delle variabili booleane
        if (booleanAccessor != null)
        {
            var convManager = ConversationManager.Instance; // Accesso al gestore delle conversazioni

            bool fioriRaccolti = booleanAccessor.GetBoolFromThis("fioriRaccolti");
            bool parlatoConGaia = booleanAccessor.GetBoolFromThis("fiori");

            // Caso 1 e Caso 3: Non ho raccolto i fiori (anche non ho parlato con gaia) o ho raccolto i fiori ma non parlato con Gaia -> dialogo1
            // if (!fioriRaccolti || (fioriRaccolti && !parlatoConGaia)) convManager.StartConversation(conversations[0]);
            if (!parlatoConGaia || (parlatoConGaia && !fioriRaccolti)) StartConversation(conversations[0]);
            else if (fioriRaccolti && parlatoConGaia) // Caso 2: Ho raccolto i fiori e ho parlato con Gaia -> dialogo2
            {
                StartConversation(conversations[1]);
                convManager.SetBool("soluzione", booleanAccessor.GetBoolFromThis("soluzione"));
            }
            convManager.SetBool("fioriRaccolti", fioriRaccolti);
            convManager.SetBool("fiori", parlatoConGaia);
        }
        else
        {
            Debug.LogError("BooleanAccessor.istance non è stato inizializzato.");
        }
    }

    protected override void EndDialogue() { }
}
