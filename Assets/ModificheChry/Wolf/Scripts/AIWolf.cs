using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIWolf : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isAttacking = false; // Flag per controllare se il lupo sta attaccando
    private bool hasEnteredTargetArea = false; // Flag per controllare se il lupo è entrato nell'area target
    private bool isRotatingToAttack = false; // Flag per controllare se il lupo sta ruotando per attaccare
    private bool firstTimeSeeingPlayer = false; // Flag per controllare se è la prima volta che il lupo vede il player
    #endregion

    [Header("References")]
    #region References
    public Diario diario; // Riferimento al diario
    public AudioClip growl; // Riferimento al suono del ringhio
    private AudioSource audioSource; // Riferimento all'audio source del lupo
    public NPCConversation dialogue; // Riferimento al dialogo del lupo
    public Transform targetEndTask; // Riferimento al punto in cui la quest termina
    private NavMeshAgent agent; // Riferimento all'Agente di navigazione del lupo
    private Transform player; // Riferimento al player
    private Animator animator; // Riferimento all'animator del lupo
    public AI wolf; // Riferimento allo scriptable object dell'AI del lupo
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = FindAnyObjectByType<Player>().transform;
    }

    void Update()
    {
        var boolAccessor = BooleanAccessor.istance;
        if (boolAccessor.GetBoolFromThis("wolfDone")) { this.enabled = false; return; } // Se la quest è completata, disabilita lo script
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh

        if (!animator.GetBool("bait") || !boolAccessor.GetBoolFromThis("wolf")) // Se il player non ha interagito con l'esca o non ha parlato con UomoBaita
        {
            if (CanAttack()) // Se può attaccare il player
            {
                isAttacking = true;
                isRotatingToAttack = true;
                animator.SetTrigger("attack"); // Durante l'animazione di attacco, verrà invocata la funzione PerformRaycastAttack
            }
            return;
        }

        if (!animator.GetBool("nearPlayer") && !boolAccessor.GetBoolFromThis("wolf")) return; // Se non gli sono vicino e non ha dialogato con uomo baita

        if (hasEnteredTargetArea) // Se il lupo è entrato nell'area target
        {
            if (!IsNear(targetEndTask, wolf.stopDistance)) return; // Se non è vicino al target, avvicinati
            ResetAtTarget(boolAccessor); // Se è vicino al target, resetta
            return;
        }
        else FollowPlayer(); // Se ha l'esca, non è vicino al target e la quest è attiva e non completata
    }

    private bool CanAttack() => 
        IsNear(player, wolf.distanceAttackMelee) && // Se il player è abbastanza vicino
        !isAttacking && // Se non sta già attaccando
        !isRotatingToAttack && // Se non sta già ruotando per attaccare
        !player.GetComponent<Player>().isDead; // Se il player non è morto

    private bool IsNear(Transform target, float distance) => Vector3.Distance(transform.position, target.position) <= distance;

    public void PerformRaycastAttack() // Invocata da attack nell'animation del lupo
    {
        if (!isAttacking) return; // Se non sta attaccando, non fare nulla

        Vector3 rayDirection = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, wolf.distanceAttackMelee)) // Se colpisce il player
        {
            hit.collider.transform.root.GetChild(0).GetComponent<Player>()?.UpdateHealth(wolf.melee1Damage);
        }
        isAttacking = false;
    }

    private void FollowPlayer()
    {
        // Ottieni la distanza tra il lupo e il player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > wolf.maxSightDistance) // Se il player è abbastanza lontano, il lupo lo segue
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
        // Breve dialogo siccome ad es UomoBaita è contento che il lupo sia arrivato: creare uno script che deriva da dialogue base e che ha una funzione (da invocare qui)
        hasEnteredTargetArea = true;
        agent.SetDestination(targetEndTask.position);
        agent.isStopped = false;
    }

    private void ResetAtTarget(BooleanAccessor boolAccessor)
    {
        animator.SetFloat("speed", 0f);
        agent.stoppingDistance = wolf.stopDistance;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.ResetPath();
        boolAccessor.SetBoolOnDialogueE("wolfDone");
        diario.CompletaMissione("Riporta il cucciolo");
    }

    private IEnumerator RotateTowardsPlayerAndAttack()
    {
        agent.isStopped = true;

        while (true)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * wolf.rotationSpeed);

            if (Quaternion.Angle(transform.rotation, lookRotation) < 5f) // Se è allineato con il player
            {
                isRotatingToAttack = false;
                break;
            }
            yield return null;
        }

        agent.isStopped = false;
    }

    private bool CanRotate(Collider other) =>
        other.CompareTag("Player") && // Se il player entra nel trigger del lupo
        (!animator.GetBool("bait") || // Se il player non ha interagito con l'esca
        !BooleanAccessor.istance.GetBoolFromThis("wolf")); // Se il player non ha parlato con UomoBaita

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Se il player entra nel trigger del lupo
        {
            animator.SetBool("nearPlayer", true);

            if (animator.GetBool("bait") && BooleanAccessor.istance.GetBoolFromThis("wolf")) return;

            if (!audioSource.isPlaying) audioSource.PlayOneShot(growl); // Suona il ringhio del lupo
            if (!firstTimeSeeingPlayer) // Se è la prima volta che il lupo vede il player
            {
                print("Primo ringhio");
                firstTimeSeeingPlayer = true;
                ConversationManager.Instance.StartConversation(dialogue);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (CanRotate(other)) StartCoroutine(RotateTowardsPlayerAndAttack());
    }

    void OnTriggerExit(Collider other)
    {
        if (CanRotate(other))
        {
            animator.SetBool("nearPlayer", false);
            audioSource.Stop();
        }
    }
}
