using UnityEngine;

public class NPCAIWithWaypoints : NPCAIBase
{
    [Header("Waypoint Settings")]
    Transform[] waypoints; // Array di waypoint
    private int currentWaypointIndex = 0; // Indice del waypoint attuale
    public float waypointThreshold = 0.5f; // Distanza minima per considerare raggiunto un waypoint

    protected override void Update()
    {
        base.Update();

        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        // Se non ci sono waypoint, non fare nulla
        if (waypoints == null || waypoints.Length == 0) return;
        if (currentWaypointIndex >= waypoints.Length) { StopAgent(); return; }

        // Muove l'agente verso il waypoint corrente
        MoveTowardsTarget(waypoints[currentWaypointIndex].position, 6.5f, 5f); // Usa una velocità costante o basata sul movimento
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0; // Resetta l'indice al primo waypoint
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaypointAI") && other.transform == waypoints[currentWaypointIndex])
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
