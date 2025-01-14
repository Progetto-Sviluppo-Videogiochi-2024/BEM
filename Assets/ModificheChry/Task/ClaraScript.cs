
public class ClaraScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        StartConversation(conversations[0]); // Avvia Clara2
    }

    protected override void EndDialogue() { }
}
