using System.Collections;
using UnityEngine;

public class NPCAIWithWaypoints : NPCAIBase
{
    [Header("Explosion")]
    bool isRotating = false; // Flag per controllare se l'AI sta ruotando verso la dinamite
    public Transform dinamite; // Riferimento al GO della dinamite

    [Header("Waypoints")]
    Transform[] waypoints; // Array di waypoint
    private int currentWaypointIndex = 0; // Indice del waypoint attuale

    protected override void Update()
    {
        if (!enabled) return; // Se l'AI è disabilitata, non fare nulla
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agent non è attivo o non è su un NavMesh, non fare nulla
        if (waypoints == null || waypoints.Length == 0) return; // Se non ci sono waypoint, non fare nulla

        if (currentWaypointIndex != waypoints.Length - 1) // Se non è l'ultimo waypoint
            MoveTowardsTarget(waypoints[currentWaypointIndex].position, 6.5f, 5f);
        else // Se deve andare all'ultimo waypoint (dinamite)
        {
            if (!isRotating) // Per far vedere che vedono la dinamite
            {
                StopAgent();
                StartCoroutine(RotateTowardsCoroutine(dinamite));
                isRotating = true;
            }
            else if (BooleanAccessor.istance.GetBoolFromThis("LandslideCollapsed")) // Se la frana è crollata, segui il player
            {
                MoveTowardsTarget(player.transform.position, 6.5f, 5f);
            }
        }
    }

    public void DisableAI()
    {
        StartCoroutine(RotateTowardsCoroutine(waypoints[^1]));
        StopAgent();
        enabled = false;
    }

    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
        currentWaypointIndex = 0; // Resetta l'indice al primo waypoint
    }

    public IEnumerator RotateTowardsCoroutine(Transform targetTransform)
    {
        while (true)
        {
            // Calcola la direzione verso il target
            Vector3 direction = (targetTransform.position - transform.position).normalized;

            // Calcola la rotazione desiderata verso il target
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Interpola gradualmente verso la rotazione desiderata
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f * Time.deltaTime);

            // Verifica se la rotazione è sufficientemente vicina alla desiderata
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation; // Allinea esattamente al target
                yield break; // Termina la Coroutine
            }

            yield return null; // Aspetta il frame successivo
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaypointAI") && other.transform == waypoints[currentWaypointIndex])
        {
            currentWaypointIndex++; // Passa al prossimo waypoint
        }
    }
}
