using UnityEngine;

public class ClaraScript : NPCDialogueBase
{
    private Animator animator; // Riferimento all'animator

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected override void StartDialogue()
    {
        var boolAccessor = BooleanAccessor.istance;

        if (!boolAccessor.GetBoolFromThis("firstMeetClara")) // Se non ha parlato con Clara per la prima volta
        {
            animator.SetBool("talk", true);
            StartConversation(conversations[0]);
        }
        else StartConversation(conversations[1]);
        // else parte la task di Clara?
    }

    protected override void EndDialogue()
    {
        animator.SetBool("talk", false);
    }
}
