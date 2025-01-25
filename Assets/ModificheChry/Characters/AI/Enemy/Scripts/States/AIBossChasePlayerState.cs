using UnityEngine;

public class AIBossChasePlayerState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        agent.PlayAudio(0, true); // Suono di inseguimento
    }

    public void Exit(AIBossAgent agent)
    {
        agent.audioSource.Stop(); // Ferma l'audio
    }

    public AIStateId GetId() => AIStateId.ChasePlayer;

    public void Update(AIBossAgent agent)
    {
        // Precondizioni necessarie
        if (agent == null || !agent.navMeshAgent.isActiveAndEnabled || !agent.navMeshAgent.isOnNavMesh)
        {
            Debug.LogError($"Agent is null ? {agent == null}\nis isOnNavMesh ? {agent.navMeshAgent.isOnNavMesh}\nis isActiveAndEnabled ? {agent.navMeshAgent.isActiveAndEnabled}");
            return;
        }
        if (!agent.enabled) return;

        if (!agent.status.IsEnemyAlive()) { agent.stateMachine.ChangeState(AIStateId.Death); return; }
        if (agent.player.IsDead()) { agent.StopAudio(); return; }
        if (agent.status.isStunned) return;

        // Calcolo della distanza per verificare se può attaccare
        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);
        if (distanceToPlayer <= agent.distanceMelee)
        {
            agent.stateMachine.ChangeState(AIStateId.Attack); // Attacco ravvicinato (melee)
            return;
        }
        else if (agent.Range(distanceToPlayer, agent.minDistanceRange, agent.maxDistanceRange))
        {
            if (agent.ShouldChase()) // 50% di probabilità
            {
                agent.stateMachine.ChangeState(AIStateId.Attack); // Attacco a distanza (range)
                return;
            }
        }

        // Movimento verso il giocatore
        if (!agent.navMeshAgent.hasPath || agent.navMeshAgent.destination != agent.player.transform.position)
        {
            agent.navMeshAgent.SetDestination(agent.player.transform.position);
        }
    }
}
