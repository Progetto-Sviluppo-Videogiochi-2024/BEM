
public class SwapState : ActionBaseState
{
    public override void EnterState(ActionStateManager action) { action.animator.SetTrigger("swapWeapon");; }

    public override void UpdateState(ActionStateManager action) { }
}
