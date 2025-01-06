using UnityEngine;
using DialogueEditor;

public class AngelicaScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        // Logica per Angelica, semplicemente avvia il dialogo
        StartConversation(conversations[0]);
    }

    protected override void EndDialogue() { }
}
