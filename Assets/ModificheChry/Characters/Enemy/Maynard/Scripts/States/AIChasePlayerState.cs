using UnityEngine;
using UnityEngine.AI;

public class AIChasePlayerState : AIState
{
    public void Enter(AIAgent agent) { }

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.ChasePlayer;

    public void Update(AIAgent agent)
    {
        if (!agent.enabled) return;

        // Calcola la distanza dal giocatore
        // float distanceToPlayer = Vector3.Distance(agent.transform.position, player.position);

        // Se il nemico è abbastanza vicino al giocatore, attaccalo
        // if (distanceToPlayer <= agent.config.attackRange)
        // {
        //     agent.stateMachine.ChangeState(AIStateId.Attack);
        //     return;
        // }

        // Se il nemico ha perso il giocatore (opzione 1: distanza troppo lunga o visibilità persa), torna al pattugliamento
        // if (distanceToPlayer > agent.config.detectionRange || !IsPlayerVisible(agent))
        // {
        //     agent.stateMachine.ChangeState(AIStateId.Patrol);
        //     return;
        // }

        // Se l'agente non ha ancora un percorso o è necessario calcolarlo di nuovo, aggiorna la destinazione
        if (!agent.navMeshAgent.hasPath || agent.navMeshAgent.destination != agent.player.position)
        {
            agent.navMeshAgent.SetDestination(agent.player.position);  // Calcola automaticamente il percorso verso il giocatore
        }
        // L'agente calcolerà il percorso ottimale per arrivare a Stefano, aumentando la difficoltà di gioco
    }
}
