using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

public class HumanFollower : MonoBehaviour
{
    [Header("Settings")]
    public float stopDistance = 2f; // Distanza a cui l'umano si ferma dal target

    [Header("References")]
    public Transform target; // Il target da inseguire (es. giocatore)
    private NavMeshAgent agent; // Agente NavMesh per il movimento
    private Animator animator; // Riferimento all'animator per gestire le animazioni
    [HideInInspector] public bool HitBall = false;
    void Start()
    {
        // Recupera i componenti necessari
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (target == null)
        {
            Debug.LogError("Target non assegnato! Assicurati di specificare un Transform da inseguire.");
        }
    }

    void Update()
    {
        if (target == null || agent == null || !agent.isOnNavMesh) return;
        if (!BooleanAccessor.istance.GetBoolFromThis(("preHitBallJA")) || !ConversationManager.Instance.hasClickedEnd) return;
        // Calcola la distanza dal target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (!HitBall && distanceToTarget > stopDistance)
        {   
            // Segui il target
            agent.SetDestination(target.position);
            agent.isStopped = false;
            //animator?.SetFloat("speed", agent.speed); // Aggiorna l'animazione di movimento
        }
        else if(HitBall)
        {   
            // Fermati se sei vicino al target
            agent.isStopped = true;
            agent.ResetPath();
           // animator?.SetFloat("speed", 0f); // Anima il fermo
        }
    }
}
