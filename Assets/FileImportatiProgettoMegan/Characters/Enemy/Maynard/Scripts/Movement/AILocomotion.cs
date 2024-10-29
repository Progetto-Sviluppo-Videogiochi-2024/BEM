using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AILocomotion : MonoBehaviour
{
    [Header("References")]
    #region References
    private NavMeshAgent agent; // Riferimento all'agente di navigazione
    private Animator animator; // Riferimento all'Animator del nemico
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (agent.hasPath) animator.SetFloat("speed", agent.velocity.magnitude);
        else animator.SetFloat("speed", 0);
    }

    // [Header("Settings")]
    // #region Settings
    // public float followDistance = 0.5f; // Distanza minima dal player
    // public float maxTime = 1.0f; // Tempo massimo tra gli aggiornamenti del path
    // public float maxDistance = 1.0f; // Distanza massima per aggiornare il percorso
    // public float slowDownDistance = 3.0f; // Distanza a cui iniziare a rallentare
    // public float runDistance = 5.0f; // Distanza a cui l'IA corre
    // public float minSpeed = 0.5f; // Velocità minima quando l'IA è molto vicina
    // public float walkSpeed = 2.0f; // Velocità di camminata
    // public float runSpeed = 5.0f; // Velocità massima (corsa)
    // private float timer = 0.0f; // Timer per gestire l'aggiornamento del path
    // #endregion
    // void Update() // Fatto da chatgpt
    // {
    //     if (agent.isActiveAndEnabled && agent.isOnNavMesh)
    //     {
    //         // Aggiorna il timer
    //         timer -= Time.deltaTime;

    //         // Calcola la distanza attuale dal player
    //         float distanceToPlayer = Vector3.Distance(transform.position, player.position);

    //         // Se l'IA è lontana, corre
    //         if (distanceToPlayer > slowDownDistance)
    //         {
    //             agent.speed = runSpeed; // Imposta la velocità di corsa
    //             agent.SetDestination(player.position);
    //         }
    //         // Se l'IA è vicina, inizia a rallentare
    //         else if (distanceToPlayer < slowDownDistance && distanceToPlayer > followDistance)
    //         {
    //             // Usa l'interpolazione per regolare la velocità di decelerazione tra camminata e corsa
    //             float t = Mathf.InverseLerp(followDistance, slowDownDistance, distanceToPlayer);
    //             agent.speed = Mathf.Lerp(minSpeed, walkSpeed, t); // Decelera gradualmente
    //             agent.SetDestination(player.position);
    //         }
    //         // Se è abbastanza vicino al player, fermati
    //         else if (distanceToPlayer <= followDistance)
    //         {
    //             agent.ResetPath(); // Ferma l'agente
    //             agent.velocity = Vector3.zero; // Assicura che la velocità sia zero
    //             animator.SetFloat("speed", 0); // Imposta l'animazione di stop (idle)
    //             return; // Esci dalla funzione per evitare aggiornamenti non necessari
    //         }

    //         // Aggiorna l'animazione in base alla velocità dell'agente
    //         animator.SetFloat("speed", agent.velocity.magnitude);
    //     }
    // }
}
