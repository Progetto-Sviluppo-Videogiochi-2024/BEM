using UnityEngine;

public class AILocomotion : NPCAIBase
{
    [Header("Settings")]
    #region Settings
    public float minDistanceToRunToPlayer = 5f; // Distanza minima per iniziare a inseguire il giocatore
    public float walkSpeed = 2f; // Velocità di camminata
    public float runSpeed = 6f; // Velocità di corsa
    [Tooltip("Velocità di transizione tra diversi tipi di speed")] public float interpolationSpeed = 5f; // Velocità di interpolazione
    #endregion

    [Header("References")]
    #region References
    AIAgent aIAgent;
    #endregion

    protected override void Start()
    {
        base.Start();
        aIAgent = GetComponent<AIAgent>();
    }

    protected override void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            Debug.LogError($"Agent is null ? {agent == null}\nis isOnNavMesh ? {agent.isOnNavMesh}\nis isActiveAndEnabled ? {agent.isActiveAndEnabled}");
            return;
        }

        // Se lo stato è Chase, muovi l'IA verso il giocatore, else non inseguirlo
        if (aIAgent.stateMachine.currentState == AIStateId.ChasePlayer)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            MoveTowardsTarget(player.position, (distanceToPlayer > minDistanceToRunToPlayer) ? runSpeed : walkSpeed, interpolationSpeed);
        }
    }
}
