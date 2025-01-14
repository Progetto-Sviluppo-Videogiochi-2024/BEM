using System.Collections;
using System.Linq;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIWolf : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool isConversationActive = false; // Flag per controllare se la conversazione è attiva
    private bool hasBait = false; // Flag per controllare se il player ha l'esca
    private bool isAttacking = false; // Flag per controllare se il lupo sta attaccando
    private bool hasEnteredTargetArea = false; // Flag per controllare se il lupo è entrato nell'area target
    private bool isRotatingToAttack = false; // Flag per controllare se il lupo sta ruotando per attaccare
    private bool firstTimeSeeingPlayer = false; // Flag per controllare se è la prima volta che il lupo vede il player
    #endregion

    [Header("Config AI")]
    #region Config AI
    [Tooltip("Distanza massima di vista")] public float maxSightDistance = 2f; // Distanza massima di vista (anche di inseguimento)
    [Tooltip("Distanza da cui l'IA si ferma")] public float stopDistance = 1.5f; // Distanza di stop (anche dal target)
    [Tooltip("Velocità di rotazione dell'IA")] public float rotationSpeed = 10f; // Velocità di rotazione
    [Tooltip("Distanza massima di attacco in mischia")] public float distanceAttackMelee = 1.5f; // Distanza massima di attacco in mischia
    [Tooltip("Danno dell'attacco da mischia1")] public int melee1Damage = -25; // Danno dell'attacco da mischia1
    #endregion

    [Header("References")]
    #region References
    public Diario diario; // Riferimento al diario
    public AudioClip growl; // Riferimento al suono del ringhio
    private AudioSource audioSource; // Riferimento all'audio source del lupo
    public NPCConversation[] conversations; // Riferimento alle conversazioni del lupo
    public Transform targetPositionStop; // Riferimento al punto in cui il lupo si ferma
    private NavMeshAgent agent; // Riferimento all'Agente di navigazione del lupo
    private Transform player; // Riferimento al player
    private Animator animator; // Riferimento all'animator del lupo
    private BooleanAccessor booleanAccessor; // Riferimento al BooleanAccessor
    private ConversationManager conversationManager; // Riferimento al ConversationManager
    #endregion

    void Start()
    {
        booleanAccessor = BooleanAccessor.istance;
        conversationManager = ConversationManager.Instance;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = FindAnyObjectByType<Player>().transform;

        PlayerPrefs.SetInt("hasBait", SaveLoadSystem.Instance.gameData.levelData.playerPrefs.Where(p => p.key == "hasBait").Select(p => p.value).FirstOrDefault());
        PlayerPrefs.Save();
    }

    void Update()
    {
        if (booleanAccessor.GetBoolFromThis("wolfDone")) { ResetWolf(); this.enabled = false; return; } // Se la quest è completata, disabilita lo script
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return; // Se l'agente non è attivo o non è sulla navmesh
        if (!animator.GetBool("nearPlayer")) return; // Se non è vicino al player
        if (!hasBait && booleanAccessor.GetBoolFromThis("wolf") && PlayerHasBait()) { hasBait = true; animator.SetBool("followPlayer", true); }

        // Se non ha l'esca o non ha parlato con UomoBaita
        if (!animator.GetBool("followPlayer") || !booleanAccessor.GetBoolFromThis("wolf"))
        {
            if (CanAttack()) // Se può attaccare il player
            {
                isAttacking = true;
                isRotatingToAttack = true;
                animator.SetTrigger("attack"); // Durante l'animazione di attacco, verrà invocata la funzione PerformRaycastAttack
                StartCoroutine(ResetIsAttackingAfterDelay(1.0f)); // 1 secondo di attesa massimo
            }
            return;
        }

        // Else se ha l'esca e ha parlato con UomoBaita
        if (hasEnteredTargetArea) // Se il lupo è entrato nell'area target
        {
            if (!IsNear(targetPositionStop, stopDistance)) return; // Se non è vicino al target, avvicinati
            ResetAtTarget(); // Se è vicino al target, resetta
            return;
        }
        else FollowPlayer(); // Se ha l'esca, non è vicino al target e ha parlato
    }

    private bool CanAttack() =>
        !player.GetComponent<Player>().isDead && // Se il player non è morto
        IsNear(player, distanceAttackMelee) && // Se il player è abbastanza vicino
        !isAttacking && // Se non sta già attaccando
        !isRotatingToAttack;// Se non sta già ruotando per attaccare // Se il player non è morto

    private bool IsNear(Transform target, float distance) => Vector3.Distance(transform.position, target.position) <= distance;

    private bool PlayerHasBait() => PlayerPrefs.GetInt("hasBait") == 1;

    public void PerformRaycastAttack() // Invocata da attack nell'animation del lupo
    {
        if (!isAttacking) return; // Se non sta attaccando, non fare nulla

        Vector3 rayDirection = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, distanceAttackMelee)) // Se colpisce il player
        {
            var player = hit.collider.transform.root.GetChild(0).GetComponent<Player>();
            player?.UpdateStatusPlayer(melee1Damage, 0);

            if (player?.health <= 0)
            {
                animator.SetBool("nearPlayer", false);
                Rigidbody rb = hit.transform.gameObject.GetComponent<Rigidbody>();
                if (hit.transform.gameObject == player.gameObject) rb = player.GetComponentInChildren<Rigidbody>();
                rb.AddForce(rayDirection * 5f, ForceMode.Impulse);
            }
        }
        isAttacking = false;
    }

    private IEnumerator ResetIsAttackingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (isAttacking && !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) isAttacking = false;
    }

    private void FollowPlayer()
    {
        // Ottieni la distanza tra il lupo e il player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > maxSightDistance) // Se il player è abbastanza lontano, il lupo lo segue
        {
            agent.speed = player.GetComponent<MovementStateManager>().currentMoveSpeed;
            animator.SetFloat("speed", agent.speed);
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
        else // Se è abbastanza vicino, il lupo si ferma
        {
            agent.speed = 0f;
            animator.SetFloat("speed", 0f);
            agent.isStopped = true;
        }
    }

    public void WolfInArea()
    {
        if (!isConversationActive) { isConversationActive = true; conversationManager.StartConversation(conversations[1]); }
        hasEnteredTargetArea = true;
        agent.SetDestination(targetPositionStop.position);
        agent.isStopped = false;
    }

    private void ResetAtTarget()
    {
        ResetWolf();
        booleanAccessor.SetBoolOnDialogueE("wolfDone");
        diario.CompletaMissione("Riporta il cucciolo");
        diario.AggiungiMissione("Spara le bottiglie");
    }

    private void ResetWolf()
    {
        animator.SetFloat("speed", 0f);
        agent.stoppingDistance = stopDistance;
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.ResetPath();
    }

    private IEnumerator RotateTowardsPlayerAndAttack()
    {
        agent.isStopped = true;

        while (true)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

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
        (!animator.GetBool("followPlayer") || // Se il player non ha interagito con l'esca
        !booleanAccessor.GetBoolFromThis("wolf")); // Se il player non ha parlato con UomoBaita

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Se il player entra nel trigger del lupo
        {
            animator.SetBool("nearPlayer", true);

            if (booleanAccessor.GetBoolFromThis("wolfDone")) return; // Se la quest è completata, non fare nulla
            if (animator.GetBool("followPlayer") && booleanAccessor.GetBoolFromThis("wolf")) return;

            if (!audioSource.isPlaying) audioSource.PlayOneShot(growl); // Suona il ringhio del lupo
            if (!firstTimeSeeingPlayer) // Se è la prima volta che il lupo vede il player
            {
                firstTimeSeeingPlayer = true;
                conversationManager.StartConversation(conversations[0]);
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

            isRotatingToAttack = false;
            isAttacking = false;
        }
    }
}
