using UnityEngine;
using DialogueEditor;

public class CollisioneMuroOrg : NPCDialogueBase
{
    protected override void StartDialogue()
    {
        // Logica per Angelica, semplicemente avvia il dialogo
        StartConversation(conversations[0]);
    }

    protected override void Update()
    {
        if (!clickEndHandled && ConversationManager.Instance.hasClickedEnd)
        {
            isConversationActive = false;
            clickEndHandled = true;

            // print("NPCDialogueBase.Update 1.if: " + gameObject.name);
            GestoreScena.ChangeCursorActiveStatus(false, "NPCDialogueBase.update: " + gameObject.transform.parent.name);
            player.GetComponent<MovementStateManager>().enabled = true;
        }

        if (isInRange && !isConversationActive)
        {
            ConversationManager.Instance.hasClickedEnd = false;
            clickEndHandled = false;
            // print("NPCDialogueBase.Update 2.if: " + gameObject.name);

            StartDialogue(); // Avvia il dialogo (metodo astratto che deve invocare StartConversation)
            player.GetComponent<MovementStateManager>().enabled = false;
        }
    }


}
