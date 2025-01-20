using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossChasePlayerState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        Debug.Log("Chase Player");
    }

    public void Exit(AIBossAgent agent)
    {
        Debug.Log("Exit Chase Player");
    }

    public AIStateId GetId()
    {
        return AIStateId.ChasePlayer;
    }

    public void Update(AIBossAgent agent)
    {
        Debug.Log("Update Chase Player");
    }
}
