using UnityEngine;
using DialogueEditor;

public class TriggerDialogueClara : NPCDialogueBase
{
    private bool hasDialogueBeenShown = false;
    [SerializeField] private Animator claraAnimator; // Riferimento all'animator di Clara

    protected override void StartDialogue()
    {
        // Attiva l'animazione di Clara
        if (claraAnimator != null)
        {
            claraAnimator.SetBool("talk", true);
        }

        StartConversation(conversations[0]);

        // Assicurati che il sistema registri correttamente la fine del dialogo
        ConversationManager.OnConversationEnded += OnDialogueEnded;
    }

    private void OnDialogueEnded()
    {
        // Rimuovi il listener per evitare chiamate multiple
        ConversationManager.OnConversationEnded -= OnDialogueEnded;

        // Disattiva l'animazione di Clara
        if (claraAnimator != null)
        {
            claraAnimator.SetBool("talk", false);
        }

        // Reimposta lo stato della conversazione e riabilita i movimenti
        isConversationActive = false;
        player.GetComponent<MovementStateManager>().enabled = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other); // Mantieni la logica della classe base (opzionale)
        if (!hasDialogueBeenShown && isInRange)
        {
            hasDialogueBeenShown = true;
            isConversationActive = true;
            player.GetComponent<MovementStateManager>().enabled = false;
            StartDialogue();
        }
    }

    protected override void EndDialogue()
    {
        // Disattiva l'animazione di Clara (in caso venga chiamato da altri eventi)
        if (claraAnimator != null)
        {
            claraAnimator.SetBool("talk", false);
        }
    }
}
