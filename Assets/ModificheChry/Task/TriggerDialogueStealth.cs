
public class TriggerDialogueStealth : TriggerDialogue
{
    protected override void Start()
    {
        base.Start();
        if (BooleanAccessor.istance.GetBoolFromThis("stealth")) gameObject.SetActive(false);
    }
}
