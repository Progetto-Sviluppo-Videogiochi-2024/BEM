using UnityEngine;
using UnityEngine.AI;

public class AIChasePlayerState : AIState
{
    [Header("Settings")]
    #region Settings
    float timer = 0.0f; // Timer per gestire l'aggiornamento del path
    #endregion

    [Header("References")]
    #region References
    public Transform player; // Riferimento al giocatore
    #endregion

    public void Enter(AIAgent agent)
    {
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.ChasePlayer;

    public void Update(AIAgent agent)
    {
        if (!agent.enabled) return;

        timer -= Time.deltaTime;
        if (!agent.navMeshAgent.hasPath) agent.navMeshAgent.destination = player.transform.position;

        if (timer <= 0.0f)
        {
            Vector3 direction = player.position - agent.navMeshAgent.destination;
            direction.y = 0;
            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                    agent.navMeshAgent.destination = player.position;
            }
            timer = agent.config.maxTime;
        }
    }
}
