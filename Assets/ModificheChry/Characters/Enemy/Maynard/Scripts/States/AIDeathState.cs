using UnityEngine;

public class AIDeathState : AIState
{
    public void Enter(AIAgent agent)
    {
        Debug.Log("Enter Death State");
        agent.player.hasEnemyDetectedPlayer = false;
        agent.ragdollManager.TriggerRagdoll();
        agent.navMeshAgent.enabled = false;
        agent.detection.enabled = false;
        agent.status.enabled = false;
        agent.locomotion.enabled = false;
        agent.animator.enabled = false;
    }

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.Death;

    public void Update(AIAgent agent) { }
}
