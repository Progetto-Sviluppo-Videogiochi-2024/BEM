using UnityEngine;
using UnityEngine.AI;

public class AIWolf : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public float followDistance; // Distanza minima per "scappare" dal giocatore
    public float safeDistance; // Distanza di sicurezza dalla quale la lupo si ferma
    public float stopThreshold; // Soglia per fermare la lupo quando è vicina alla destinazione
    private bool hasEnteredTargetArea = false; // Flag per controllare se la lupo è entrata nell'area target
    #endregion

    [Header("References")]
    #region References
    public Transform targetEndTask; // Punto in cui la quest termina
    private NavMeshAgent agent; // Agente di navigazione della lupo
    private Transform player; // Riferimento al giocatore
    #endregion

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) return;
        // if (pg fa spacebar vicino alla lupo && quest non attiva) {lupo fa "beee" (SFX) + return;} // Quindi se pg non ha dialogato con UomoBaita
        // if (quest non attiva) return; // Se la quest non è attiva, la lupo non si muove
        // Se la lupo è vicina al target finale, si ferma
        if (hasEnteredTargetArea /*&& quest è completata*/)
        {
            if (!IsNearTarget()) return;

            agent.stoppingDistance = stopThreshold;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.ResetPath();
            return;
        }
        else FollowPlayer();
    }

    private bool IsNearTarget() => Vector3.Distance(transform.position, targetEndTask.position) <= stopThreshold;

    private void FollowPlayer()
    {
        // Imposta la velocità dell'agente in base alla velocità di movimento attuale del giocatore
        agent.speed = player.GetComponent<MovementStateManager>().currentMoveSpeed;

        // Ottieni la distanza tra il lupo e il giocatore
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Se il giocatore è abbastanza lontano, il lupo lo segue
        if (distanceToPlayer > followDistance)
        {
            agent.SetDestination(player.position);
            agent.isStopped = false;
        }
        else agent.isStopped = true;

        // Aggiungere l'animazione di camminata del lupo qui (es. lupo.animator.speed -> blend tree)
    }

    public void WolfInArea()
    {
        if (targetEndTask != null)
        {
            // BooleanAccessor.istance.SetBoolOnDialogueE("wolfIsInArea", true);
            // quest = completata
            // lupo fa "beee" (SFX) appena entra nell'area
            // Al max da qui potresti far partire un breve dialogo (dovrebbe entrare solo una volta in questa funzione -> siccome in target questa funzione invocata in enter)
            hasEnteredTargetArea = true;
            agent.SetDestination(targetEndTask.position);
            agent.isStopped = false;
        }
    }
}
