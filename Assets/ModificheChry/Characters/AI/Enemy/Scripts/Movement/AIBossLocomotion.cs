using UnityEngine;

public class AIBossLocomotion : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public float stopDistance = 1.75f; // Distanza a cui l'agente si ferma dal target
    public float minDistanceToRunToPlayer = 5f; // Distanza minima per iniziare a inseguire il giocatore
    public float walkSpeed = 2f; // Velocità di camminata
    public float runSpeed = 6f; // Velocità di corsa
    [Tooltip("Velocità di transizione tra diversi tipi di speed")] public float interpolationSpeed = 5f; // Velocità di interpolazione
    #endregion

    [Header("References")]
    #region References
    Player player; // Riferimento al player
    Animator animator; // Riferimento all'animator
    AIBossAgent agent; // Riferimento all'agente
    #endregion

    void Start()
    {
        agent = GetComponent<AIBossAgent>();
        player = agent.player;
        animator = agent.animator;
    }

    void Update()
    {
        if (!agent.status.IsEnemyAlive() || agent.status.isStunned) { StopAgent(); return; }
        if (agent.navMeshAgent == null || !agent.navMeshAgent.isActiveAndEnabled || !agent.navMeshAgent.isOnNavMesh)
        {
            Debug.LogError($"Agent is null ? {agent.navMeshAgent == null}\n" +
                $"is isOnNavMesh ? {agent.navMeshAgent.isOnNavMesh}\n" +
                $"is isActiveAndEnabled ? {agent.navMeshAgent.isActiveAndEnabled}");
            return;
        }

        // Se lo stato è Chase, muovi l'IA verso il giocatore, else non inseguirlo
        if (agent.stateMachine.currentState == AIStateId.ChasePlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.gameObject.transform.position);
            if (distanceToPlayer <= stopDistance) { StopAgent(); return; }
            MoveTowardsTarget(player.gameObject.transform.position, (distanceToPlayer > minDistanceToRunToPlayer) ? runSpeed : walkSpeed, interpolationSpeed);
        }
    }

    void MoveTowardsTarget(Vector3 destination, float speed, float interpolationSpeed) =>
        MoveAgent(destination, Mathf.Lerp(agent.navMeshAgent.speed, speed, Time.deltaTime * interpolationSpeed)); // Interpolazione graduale

    void MoveAgent(Vector3 destination, float speed)
    {
        agent.navMeshAgent.speed = speed;
        agent.navMeshAgent.SetDestination(destination);
        agent.navMeshAgent.isStopped = false;
        animator.SetFloat("speed", speed);
    }

    void StopAgent()
    {
        agent.navMeshAgent.isStopped = true;
        agent.navMeshAgent.ResetPath();
        agent.navMeshAgent.velocity = Vector3.zero;
        agent.navMeshAgent.speed = 0f;
        animator.SetFloat("speed", Mathf.Lerp(animator.GetFloat("speed"), 0f, Time.deltaTime * 5f));
    }
}
