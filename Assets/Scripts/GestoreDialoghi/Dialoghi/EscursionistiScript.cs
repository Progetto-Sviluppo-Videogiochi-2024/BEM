using DialogueEditor;

public class EscursionistiScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        StartConversation(conversations[0]);
    }

    protected override void EndDialogue() { }
}
