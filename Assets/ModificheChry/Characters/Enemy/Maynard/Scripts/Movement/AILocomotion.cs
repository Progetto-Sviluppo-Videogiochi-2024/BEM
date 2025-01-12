using UnityEngine;

public class AILocomotion : NPCAIBase
{
    [Header("Settings")]
    public float minDistance = 5f; // Distanza minima per camminare
    public float walkSpeed = 2f; // Velocità di camminata
    public float runSpeed = 6f; // Velocità di corsa
    [Tooltip("Velocità di transizione tra diversi tipi di speed")] public float interpolationSpeed = 5f; // Velocità di interpolazione

    protected override void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh)
        {
            Debug.LogError($"Agent is null ? {agent == null}\nis isOnNavMesh ? {agent.isOnNavMesh}\nis isActiveAndEnabled ? {agent.isActiveAndEnabled}");
            return;
        }

        if (GetComponent<AIAgent>().stateMachine.currentState != AIStateId.ChasePlayer) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (target != null) MoveTowardsTarget(player.position, (distanceToPlayer > minDistance) ? runSpeed : walkSpeed, interpolationSpeed);
    }
}
