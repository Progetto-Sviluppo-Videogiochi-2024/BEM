
public class SwapState : ActionBaseState
{
    public override void EnterState(ActionStateManager action)
    {
        action.animator.SetTrigger("SwapWeapon");
        action.leftHandIK.weight = 0;
        action.rightHandAim.weight = 0;
    }

    public override void UpdateState(ActionStateManager action) { }
}
