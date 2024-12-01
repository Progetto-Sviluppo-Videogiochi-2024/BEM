
public class SwapState : ActionBaseState
{
    public override void EnterState(ActionStateManager action)
    {
        action.animator.SetTrigger("SwapWeapon");
    }

    public override void UpdateState(ActionStateManager action) { }
}
