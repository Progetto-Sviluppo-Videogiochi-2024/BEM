using UnityEngine;
using UnityEngine.AI;

public class NPCAIBase : MonoBehaviour
{
    [Header("References")]
    #region References
    protected Transform player; // Riferimento al player (target dell'agente)
    protected NavMeshAgent agent; // Riferimento all'agente di navigazione
    protected Animator animator; // Riferimento all'animator dell'agente
    [SerializeField] protected Transform target; // Riferimento al target dell'agente
    #endregion

    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        FollowPlayer();
    }

    private void FollowPlayer()
    {
        // Ottieni la distanza tra l'agente e il player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 1.85f) // Se il player è abbastanza lontano, l'agente si muove verso di lui
        {
            agent.speed = player.GetComponent<MovementStateManager>().currentMoveSpeed;
            animator.SetFloat("speed", agent.speed);
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
        else // Se è abbastanza vicino, l'agente si ferma
        {
            agent.velocity = Vector3.zero;
            agent.speed = 0f;
            animator.SetFloat("speed", 0f);
            agent.isStopped = true;
        }
    }
}
