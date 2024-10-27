
public class ReloadState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {
        actions.rightHandAim.weight = 0;
        actions.leftHandIK.weight = 0;
        actions.animator.SetTrigger("reloading");
    }

    public override void UpdateState(ActionStateManager actions) {}
}
