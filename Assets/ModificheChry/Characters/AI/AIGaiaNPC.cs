
public class AIGaiaNPC : NPCAIBase
{
    protected override void Update()
    {
        base.Update();
        animator.SetBool("crouching", player.hasEnemyDetectedPlayer);
    }
}
