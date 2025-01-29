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
    public Player player; // Riferimento al player
    protected MovementStateManager movementStateManager; // Riferimento allo stato di movimento del player
    protected NavMeshAgent agent; // Riferimento all'agente di navigazione
    protected Animator animator; // Riferimento all'animator dell'agente
    [SerializeField] public Transform target; // Riferimento al target dell'agente
    #endregion

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        movementStateManager = player.GetComponent<MovementStateManager>();

        agent.stoppingDistance = stopDistance;
    }

    protected virtual void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        if (target != null && !player.hasEnemyDetectedPlayer) MoveTowardsTarget(target.position, movementStateManager.currentMoveSpeed <= 2f ? 2f : 5f, 5f);
    }

    public virtual void MoveTowardsTarget(Vector3 destination, float speed, float interpolationSpeed)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, destination);

        // Se è vicino alla distanza di stop, ferma l'agente
        if (distanceToPlayer <= stopDistance)
        {
            if (!agent.isStopped) StopAgent();
            return;
        }
        float targetSpeed = speed;
        MoveAgent(destination, Mathf.Lerp(agent.speed, targetSpeed, Time.deltaTime * interpolationSpeed)); // Interpolazione graduale
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
        agent.velocity = Vector3.zero;
        agent.speed = 0f;
        animator?.SetFloat("speed", 0f); //Mathf.Lerp(animator.GetFloat("speed"), 0f, Time.deltaTime * 5f)); questo causa il glitch quando si ferma
    }
}
