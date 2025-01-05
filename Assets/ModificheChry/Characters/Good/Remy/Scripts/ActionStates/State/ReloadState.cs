
public class ReloadState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions) { actions.animator.SetTrigger("reloading"); }

    public override void UpdateState(ActionStateManager actions) { }
}
