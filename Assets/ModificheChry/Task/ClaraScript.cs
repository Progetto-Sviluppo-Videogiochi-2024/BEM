using UnityEngine;

public class ClaraScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        StartConversation(conversations[0]);
    }

    protected override void EndDialogue() { }

}
