using UnityEngine;
using DialogueEditor;

public class TriggerDialogue : NPCDialogueBase
{
    protected bool hasDialogueBeenShown = false; // Flag per evitare che il dialogo venga mostrato più volte

    protected override void StartDialogue()
    {
        StartConversation(conversations[0]);

        // Assicurati che il sistema registri correttamente la fine del dialogo
        ConversationManager.OnConversationEnded += OnDialogueEnded;
    }

    protected virtual void OnDialogueEnded()
    {
        // Rimuovi il listener per evitare chiamate multiple
        ConversationManager.OnConversationEnded -= OnDialogueEnded;

        // Reimposta lo stato della conversazione e riabilita i movimenti
        isConversationActive = false;
        player.GetComponent<MovementStateManager>().enabled = true;
        GestoreScena.ChangeCursorActiveStatus(false, "TriggerDialogue by " + gameObject.name);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other); // Mantieni la logica della classe base (opzionale)
        if (!playerScript.hasEnemyDetectedPlayer && !hasDialogueBeenShown && isInRange)
        {
            hasDialogueBeenShown = true;
            isConversationActive = true;
            player.GetComponent<MovementStateManager>().enabled = false;
            player.GetComponent<AudioSource>().Stop();
            StartDialogue();
        }
    }

    protected override void EndDialogue() { }
}
