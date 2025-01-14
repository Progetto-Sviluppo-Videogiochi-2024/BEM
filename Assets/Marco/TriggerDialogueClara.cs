using UnityEngine;

public class TriggerDialogueClara : TriggerDialogue
{
    [SerializeField] private Animator claraAnimator; // Riferimento all'animator di Clara

    protected override void StartDialogue()
    {
        // Attiva l'animazione di Clara
        claraAnimator?.SetBool("talk", true);
        base.StartDialogue();
    }

    protected override void EndDialogue()
    {
        claraAnimator?.SetBool("talk", false);
    }
}
