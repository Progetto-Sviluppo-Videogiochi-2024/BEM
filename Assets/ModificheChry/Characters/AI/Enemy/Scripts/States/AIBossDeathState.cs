using UnityEngine;

public class AIBossDeathState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        Debug.Log("Death");
    }

    public void Exit(AIBossAgent agent)
    {
        Debug.Log("");
    }

    public AIStateId GetId()
    {
        return AIStateId.Death;
    }

    public void Update(AIBossAgent agent)
    {
        Debug.Log("Update Death");
    }
}
