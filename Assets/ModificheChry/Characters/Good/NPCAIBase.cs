using UnityEngine;
using UnityEngine.AI;

public class NPCAIBase : MonoBehaviour
{
    [Header("References")]
    #region References
    public Transform player; // Riferimento al player (target dell'agente)
    private MovementStateManager movementStateManager; // Riferimento allo stato di movimento del player
    protected NavMeshAgent agent; // Riferimento all'agente di navigazione
    protected Animator animator; // Riferimento all'animator dell'agente
    [SerializeField] protected Transform target; // Riferimento al target dell'agente
    #endregion

    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        movementStateManager = player.GetComponent<MovementStateManager>();
    }

    protected void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        FollowTarget();
    }

    private void FollowTarget()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > 1.75f) // Se il player è abbastanza lontano, l'agente si muove verso di lui
        {
            float targetSpeed = movementStateManager.currentMoveSpeed <= 2f ? 2f : 5f;
            agent.speed = Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * 5f); // Interpolazione graduale
            animator.SetFloat("speed", agent.speed);
            agent.SetDestination(target.position);
            agent.isStopped = false;
        }
        else // Se è abbastanza vicino, l'agente si ferma
        {
            agent.speed = Mathf.Lerp(agent.speed, 0f, Time.deltaTime * 5f); // Interpolazione graduale verso zero
            animator.SetFloat("speed", agent.speed);
            agent.isStopped = true;
        }
    }
}
