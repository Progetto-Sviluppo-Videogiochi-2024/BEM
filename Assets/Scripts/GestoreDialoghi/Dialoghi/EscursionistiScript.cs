using DialogueEditor;

public class EscursionistiScript : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        ConversationManager.Instance.StartConversation(conversations[0]);
    }
}
