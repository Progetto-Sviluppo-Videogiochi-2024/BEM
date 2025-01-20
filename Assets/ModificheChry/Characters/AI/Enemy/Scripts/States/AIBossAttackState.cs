using UnityEngine;

public class AIBossAttackState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        Debug.Log("Attack");
    }

    public void Exit(AIBossAgent agent)
    {
        Debug.Log("");
    }

    public AIStateId GetId()
    {
        return AIStateId.Attack;
    }

    public void Update(AIBossAgent agent)
    {
        Debug.Log("Update Attack");
    }
}
