using UnityEngine;
using UnityEngine.AI;

// Comportamento in cui il nemico si muove in modo casuale o predeterminato all'interno di un'area, senza un obiettivo preciso (come inseguire o attaccare).
// Simula un'azione di "sorveglianza" o "pattugliamento", durante il quale il nemico esplora e si sposta a caso, finché non rileva il giocatore o
// non raggiunge una destinazione.
public class AIPatrolState : AIState<AIAgent>
{
    protected Vector3 patrolDestination; // Destinazione di pattugliamento
    protected Collider patrolAreaCollider; // Riferimento al Collider dell'area di pattugliamento

    public void Enter(AIAgent agent)
    {
        SetPatrolArea(agent.patrolArea);
        SetRandomPatrolDestination(agent);
        agent.PlayAudio(0, true);
        if (agent.gameObject.name.Contains("Scarnix")) agent.mutantAttack.hasPerformedFlyKick = false;
    }

    public void Exit(AIAgent agent) { }

    public AIStateId GetId() => AIStateId.Patrol;

    public void Update(AIAgent agent)
    {
        if (agent.player.IsDead()) { agent.StopAudio(); return; } // Se il player è morto, non fare nulla

        bool isInsidePatrolArea = patrolAreaCollider.bounds.Contains(new(agent.transform.position.x, patrolAreaCollider.bounds.center.y, agent.transform.position.z));
        if (agent.detection.enemyInDetectionRange) // Se rileva o sente il giocatore
        {
            agent.stateMachine.ChangeState(AIStateId.ChasePlayer);
            return;
        }
        else if (!isInsidePatrolArea) // Se esce dall'area di pattugliamento o è fuori dall'area di Patrol
        {
            patrolDestination = GetRandomPointInCollider(agent, patrolAreaCollider); // Calcola una nuova destinazione valida
            agent.navMeshAgent.SetDestination(patrolDestination); // Forza il rientro
            agent.locomotion.MoveTowardsTarget(patrolDestination, agent.locomotion.walkSpeed, 5f);
            return; // Evita ulteriori aggiornamenti in questo frame
        }
        else if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < 1f) SetRandomPatrolDestination(agent); // Se raggiunge la destinazione, ne calcola una nuova
        else Patrol(agent); // Segue quello specifico pattern di movimento
    }

    public void SetPatrolArea(Collider areaCollider) => patrolAreaCollider = areaCollider;

    private void Patrol(AIAgent agent)
    {
        if (Vector3.Distance(agent.transform.position, patrolDestination) < 2f)
        {
            // Vector3 oldDestination = patrolDestination; // Conserva la destinazione precedente per il log
            SetRandomPatrolDestination(agent);
        }
        agent.navMeshAgent.SetDestination(patrolDestination);
    }

    protected virtual void SetRandomPatrolDestination(AIAgent agent)
    {
        Vector3 newDestination;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            newDestination = GetRandomPointInCollider(agent, patrolAreaCollider);
            attempts++;
        }
        while (Vector3.Distance(agent.navMeshAgent.destination, newDestination) < 2f && attempts < maxAttempts);

        if (attempts >= maxAttempts) return;

        agent.navMeshAgent.SetDestination(newDestination);
        patrolDestination = newDestination;
        agent.locomotion.MoveTowardsTarget(patrolDestination, agent.locomotion.walkSpeed, 5f);
    }

    protected virtual Vector3 GetRandomPointInCollider(AIAgent agent, Collider collider)
    {
        const int maxAttempts = 20;
        for (int i = 0; i < maxAttempts; i++)
        {
            // Generazione casuale di un punto all'interno del collider
            float randomX = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
            float randomZ = Random.Range(collider.bounds.min.z, collider.bounds.max.z);
            float randomY = collider.bounds.min.y;

            Vector3 randomPoint = new(randomX, randomY, randomZ);

            // Campionamento del punto nella NavMesh
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas)) return hit.position;
        }

        // Fallback al centro del collider se non si trovano punti validi
        return collider.bounds.center;
    }
}
