using UnityEngine;
using DialogueEditor;

public class CollisioneMuroOrg : NPCDialogueBase
{
    private bool hasDialogueBeenShown = false;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other); // Mantieni la logica della classe base (opzionale)
        if (!hasDialogueBeenShown && other.CompareTag("Player"))
        {
            hasDialogueBeenShown = true;
            isConversationActive = true;
            player.GetComponent<MovementStateManager>().enabled = false;
            StartDialogue();
        }
    }


    protected override void StartDialogue()
    {
        StartConversation(conversations[0]);

        // Assicurati che il sistema registri correttamente la fine del dialogo
        ConversationManager.OnConversationEnded += OnDialogueEnded;
    }

    private void OnDialogueEnded()
    {
        // Rimuovi il listener per evitare chiamate multiple
        ConversationManager.OnConversationEnded -= OnDialogueEnded;

        // Reimposta lo stato della conversazione e riabilita i movimenti
        isConversationActive = false;
        player.GetComponent<MovementStateManager>().enabled = true;
    }
}
