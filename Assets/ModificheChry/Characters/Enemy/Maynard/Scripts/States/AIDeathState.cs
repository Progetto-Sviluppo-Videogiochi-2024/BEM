
public class AIDeathState : AIState
{
    public void Enter(AIAgent agent)
    {
        // Status dell'agente
        agent.status.isDead = true;
        agent.id = GestoreScena.GenerateId(agent.gameObject, agent.transform);
        agent.player.hasEnemyDetectedPlayer = false;
        agent.ragdollManager.TriggerRagdoll();

        // Disattiva i componenti dell'agente
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
