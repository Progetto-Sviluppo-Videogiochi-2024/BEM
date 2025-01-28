
public class TriggerDialogueBoss : TriggerDialogue
{
    public ZonaBoss zonaBoss; // Riferimento alla zona boss

    protected override void OnDialogueEnded()
    {
        base.OnDialogueEnded();
        zonaBoss.EndEntryBoss();
    }

    protected override void EndDialogue() { }
}
