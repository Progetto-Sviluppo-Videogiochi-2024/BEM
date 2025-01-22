using UnityEngine;

public class AIBossChasePlayerState : AIState<AIBossAgent>
{
    public void Enter(AIBossAgent agent)
    {
        Debug.Log("ChasePlayer");
        agent.PlayAudio(0, true); // Suono di inseguimento
    }

    public void Exit(AIBossAgent agent)
    {
        agent.audioSource.Stop(); // Ferma l'audio
    }

    public AIStateId GetId() => AIStateId.ChasePlayer;

    public void Update(AIBossAgent agent)
    {
        if (agent.player.IsDead()) { agent.StopAudio(); return; }
        if (!agent.enabled) return;

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.player.transform.position);
        Debug.Log($"Distance to player: {distanceToPlayer}");

        if (distanceToPlayer <= agent.distanceMelee)
        {
            Debug.Log("Player is near, attack!");
            agent.stateMachine.ChangeState(AIStateId.Attack); // Attacco ravvicinato (melee)
            return;
        }

        // if (distanceToPlayer > agent.minDistanceRange && distanceToPlayer <= agent.maxDistanceRange)
        // {
        //     if (Random.value > 0.5f) // 50% di probabilità
        //     {
        //         agent.stateMachine.ChangeState(AIStateId.Attack); // Attacco a distanza (range)
        //         return;
        //     }
        // }

        // Movimento verso il giocatore
        if (!agent.navMeshAgent.hasPath || agent.navMeshAgent.destination != agent.player.transform.position)
        {
            agent.navMeshAgent.SetDestination(agent.player.transform.position);
        }
    }
}
