using UnityEngine;
using UnityEngine.AI;

// Questa classe rappresenta lo stato di pattugliamento specifico per Scarnix. In quanto, in quello di default (Ghoulant), dava problemi perché essendo vicino a delle case,
// il nemico si bloccava e non sapeva dove andare.
public class AIPatrolScarnix : AIPatrolState
{
    private Collider[] overlapResults = new Collider[10]; // Array pre-allocato per i risultati

    protected override Vector3 GetRandomPointInCollider(AIAgent agent, Collider collider)
    {
        const int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(collider.bounds.min.x, collider.bounds.max.x);
            float randomZ = Random.Range(collider.bounds.min.z, collider.bounds.max.z);
            float randomY = collider.bounds.min.y;

            Vector3 randomPoint = new(randomX, randomY, randomZ);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                if (!IsNearNavMeshObstacle(hit.position, 1.5f)) // Controlla che il punto non sia vicino a ostacoli
                    return hit.position;
            }
        }

        return collider.bounds.center;
    }

    protected override void SetRandomPatrolDestination(AIAgent agent)
    {
        Vector3 newDestination;
        int attempts = 0;
        const int maxAttempts = 10;

        do
        {
            newDestination = GetRandomPointInCollider(agent, patrolAreaCollider);
            attempts++;
        }
        while ((Vector3.Distance(agent.navMeshAgent.destination, newDestination) < 2f || IsNearNavMeshObstacle(newDestination, 1.5f)) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning($"{agent.name} non è riuscito a trovare una destinazione valida.");
            return;
        }

        agent.navMeshAgent.SetDestination(newDestination);
        patrolDestination = newDestination;
        agent.locomotion.MoveTowardsTarget(patrolDestination, agent.locomotion.walkSpeed, 5f);
        Debug.DrawLine(agent.transform.position, patrolDestination, Color.yellow, 1.5f);
        Debug.Log($"{agent.name} nuova destinazione di pattugliamento: {patrolDestination}");
    }

    private bool IsNearNavMeshObstacle(Vector3 position, float radius)
    {
        // Riutilizzo dell'array pre-allocato per evitare allocazioni
        int hitCount = Physics.OverlapSphereNonAlloc(position, radius, overlapResults);

        if (hitCount == overlapResults.Length)
            Debug.LogWarning("Array overlapResults pieno. Potresti perdere alcuni ostacoli. Considera di aumentare la dimensione dell'array.");

        for (int i = 0; i < hitCount; i++)
        {
            if (overlapResults[i].GetComponent<NavMeshObstacle>() != null)
            {
                Debug.Log($"Punto vicino a un ostacolo: {overlapResults[i].name}");
                return true; // Ostacolo vicino trovato
            }
        }

        return false; // Nessun ostacolo trovato
    }
}
