using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIWolf : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public float followDistance; // Distanza minima per "scappare" dal giocatore
    public float safeDistance; // Distanza di sicurezza dalla quale il lupo si ferma
    public float stopThreshold; // Soglia per fermare il lupo quando è vicino al target
    private bool hasEnteredTargetArea = false; // Flag per controllare se il lupo è entrato nell'area target
    private bool isRotatingToAttack = false; // Flag per controllare se il lupo sta ruotando per attaccare
    public float rotationSpeed = 5f; // Velocità della rotazione per puntare il giocatore
    private bool firstTimeSeeingPlayer = false; // Flag per controllare se è la prima volta che il lupo vede il giocatore
    #endregion

    [Header("References")]
    #region References
    public AudioClip growl; // Riferimento al suono del ringhio
    private AudioSource audioSource; // Riferimento all'audio source del lupo
    public NPCConversation dialogue; // Riferimento al dialogo del lupo
    public Transform targetEndTask; // Punto in cui la quest termina
    private NavMeshAgent agent; // Agente di navigazione della lupo
    private Transform player; // Riferimento al giocatore
    private Animator animator; // Riferimento all'animator della lupo
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        // if (quest = completata) this.enabled = false; // Se la quest è completata, disattiva lo script (non avrebbe senso eseguire il resto del codice)

        if (!animator.GetBool("bait")) // Se il player non ha interagito con l'esca
        {
            if (IsNear(player, safeDistance / 2)) // Se è troppo vicino al lupo
            {
                if (!isRotatingToAttack)
                {
                    isRotatingToAttack = true;
                    animator.SetTrigger("attack");
                    // TODO: implementa il danno al giocatore (fare come per maynard)
                }
                return;
            }
        }

        if (!animator.GetBool("nearPlayer") /*&& quest non attiva*/) return; // Se non gli sono vicino e la quest non è attiva (non ha dialogato con UomoBaita), ringhia e attacca solo)

        if (hasEnteredTargetArea) // Se il lupo è entrato nell'area target
        {
            if (!IsNear(targetEndTask, stopThreshold)) return; // Se non è vicino al target, avvicinati

            animator.SetFloat("speed", 0f);
            agent.stoppingDistance = stopThreshold;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.ResetPath();
            // quest = completata
            return;
        }
        else FollowPlayer(); // Se ha l'esca, non è vicino al target e la quest è attiva e non completata
    }

    private bool IsNear(Transform target, float distance) => Vector3.Distance(transform.position, target.position) <= distance;

    private IEnumerator RotateTowardsPlayerAndAttack()
    {
        agent.isStopped = true;

        while (true)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Controlla se il lupo è allineato con il giocatore
            if (Quaternion.Angle(transform.rotation, lookRotation) < 5f)
            {
                isRotatingToAttack = false;
                break;
            }
            yield return null;
        }

        agent.isStopped = false;
    }

    private void FollowPlayer()
    {
        // Ottieni la distanza tra il lupo e il giocatore
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Se il giocatore è abbastanza lontano, il lupo lo segue
        if (distanceToPlayer > followDistance)
        {
            animator.SetFloat("speed", player.GetComponent<MovementStateManager>().currentMoveSpeed);
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
        else // Se è abbastanza vicino, il lupo si ferma
        {
            agent.isStopped = true;
            animator.SetFloat("speed", 0f);
        }
    }

    public void WolfInArea()
    {
        // BooleanAccessor.istance.SetBoolOnDialogueE("wolfIsInArea", true);
        // lupo potrebbe "ululare" (SFX) appena entra nell'area
        // Al max da qui potresti far partire un breve dialogo siccome UomoBaita è contento che il lupo sia arrivato
        hasEnteredTargetArea = true;
        agent.SetDestination(targetEndTask.position);
        agent.isStopped = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetBool("nearPlayer", true);

            if (animator.GetBool("bait")) return;
            
            if (!audioSource.isPlaying) audioSource.PlayOneShot(growl);
            if (!firstTimeSeeingPlayer)
            {
                firstTimeSeeingPlayer = true;
                ConversationManager.Instance.StartConversation(dialogue);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !animator.GetBool("bait")) StartCoroutine(RotateTowardsPlayerAndAttack());
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !animator.GetBool("bait"))
        {
            animator.SetBool("nearPlayer", false);
            audioSource.Stop();
        }
    }
}
