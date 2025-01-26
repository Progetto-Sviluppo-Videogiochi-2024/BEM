
public class AIGaiaNPC : NPCAIBase
{
    protected override void Update()
    {
        base.Update();

        if (!BooleanAccessor.istance.GetBoolFromThis("bossAppeared"))
            animator.SetBool("crouching", player.hasEnemyDetectedPlayer);
    }
}
