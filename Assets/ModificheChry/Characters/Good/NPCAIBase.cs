using UnityEngine;
using UnityEngine.AI;

public class NPCAIBase : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public float stopDistance = 1.75f; // Distanza a cui l'agente si ferma dal target
    #endregion

    [Header("References")]
    #region References
    public Transform player; // Riferimento al player
    protected MovementStateManager movementStateManager; // Riferimento allo stato di movimento del player
    protected NavMeshAgent agent; // Riferimento all'agente di navigazione
    protected Animator animator; // Riferimento all'animator dell'agente
    [SerializeField] protected Transform target; // Riferimento al target dell'agente
    #endregion

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        movementStateManager = player.GetComponent<MovementStateManager>();
    }

    protected virtual void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        if (target != null) MoveTowardsTarget(target.position);
    }

    protected virtual void MoveTowardsTarget(Vector3 destination)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, destination);

        if (distanceToPlayer > stopDistance) // Se il target è abbastanza lontano, l'agente si muove verso di lui
        {
            float targetSpeed = movementStateManager.currentMoveSpeed <= 2f ? 2f : 5f;
            MoveAgent(target.position, Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * 5f)); // Interpolazione graduale
        }
        else StopAgent(); // Se è abbastanza vicino, l'agente si ferma
    }

    protected void MoveAgent(Vector3 destination, float speed)
    {
        agent.speed = speed;
        agent.SetDestination(destination);
        agent.isStopped = false;
        animator?.SetFloat("speed", speed);
    }

    protected void StopAgent()
    {
        agent.isStopped = true;
        agent.ResetPath();
        animator?.SetFloat("speed", Mathf.Lerp(animator.GetFloat("speed"), 0f, Time.deltaTime * 5f));
    }
}
