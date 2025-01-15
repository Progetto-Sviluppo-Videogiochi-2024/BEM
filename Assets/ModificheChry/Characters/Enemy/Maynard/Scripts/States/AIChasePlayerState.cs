
using UnityEngine;

public class AIChasePlayerState : AIState
{
    public void Enter(AIAgent agent) => agent.StartCoroutine(agent.PlayNextAudio(1));

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.ChasePlayer;

    public void Update(AIAgent agent)
    {
        if (agent.player.IsDead()) { agent.StopAudio(); return; } // Se il player è morto, non fare nulla
        if (!agent.enabled) return;

        // Se il nemico è abbastanza vicino al giocatore, attaccalo
        if (Vector3.Distance(agent.transform.position, agent.player.transform.position) <= agent.minDistanceAttack)
        {
            agent.stateMachine.ChangeState(AIStateId.Attack);
            return;
        }

        // Se il nemico ha perso il giocatore (opzione 1: distanza troppo lunga o visibilità persa), torna al pattugliamento
        if (!agent.player.hasEnemyDetectedPlayer)
        {
            agent.stateMachine.ChangeState(AIStateId.Patrol);
            return;
        }

        // Se l'agente non ha ancora un percorso o è necessario calcolarlo di nuovo, aggiorna la destinazione
        if (!agent.navMeshAgent.hasPath || agent.navMeshAgent.destination != agent.player.transform.position)
        {
            agent.navMeshAgent.SetDestination(agent.player.transform.position);  // Calcola automaticamente il percorso verso il giocatore
        }
        // L'agente calcolerà il percorso ottimale per arrivare a Stefano, aumentando la difficoltà di gioco
    }
}
