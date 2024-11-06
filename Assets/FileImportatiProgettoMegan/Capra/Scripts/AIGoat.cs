using UnityEngine;
using UnityEngine.AI;

public class AIGoat : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public float followDistance; // Distanza minima per "scappare" dal giocatore
    public float safeDistance; // Distanza di sicurezza dalla quale la pecora si ferma
    public float stopThreshold; // Soglia per fermare la pecora quando è vicina alla destinazione
    private bool hasEnteredTargetArea = false; // Flag per controllare se la pecora è entrata nell'area target
    #endregion

    [Header("References")]
    #region References
    public Transform targetEndTask; // Punto in cui la quest termina
    private NavMeshAgent agent; // Agente di navigazione della pecora
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
        //QUi
        // if (quest non è attiva) return; // Se la quest non è attiva, la pecora non si muove
        // magari se non è attiva e il pg interagisce con la pecora, la pecora potrebbe fare "beee" (SFX)
        // almeno non sta lì a cazzo di cane, che fa tipo la mummia ahahah

        // Se la pecora è vicina al target finale, si ferma
        if (hasEnteredTargetArea /*&& Qui la quest è completata*/)
        {
            if (!IsNearTarget()) return;

            agent.stoppingDistance = stopThreshold;
            agent.velocity = Vector3.zero;
            agent.isStopped = true;
            agent.ResetPath();
            // Aggiungere un'animazione di camminata per la pecora (non qui però)
            // All'if + un &&: se la quest è completata
            // Altri suggerimenti -> in sheepInArea() (scendere giù)
            // Impostare l'update sse la quest è attiva quindi se ha dialogato con UomoBaita
            return;
        }
        else EscapeAwayFromPlayer();
    }

    private bool IsNearTarget()
    {
        if (targetEndTask == null)
        {
            Debug.LogWarning("TargetEndTask don't setted, targetEndTask = " + targetEndTask);
            return false;
        }

        // Verifica se la pecora è vicina al target finale
        return Vector3.Distance(transform.position, targetEndTask.position) <= stopThreshold;
    }

    private void EscapeAwayFromPlayer()
    {
        agent.speed = player.GetComponent<MovementStateManager>().currentMoveSpeed;

        // Se il giocatore è troppo vicino
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < followDistance)
        {
            // Calcola la direzione opposta al giocatore
            Vector3 awayDirection = (transform.position - player.position).normalized;

            // Calcola la posizione verso cui scappare
            Vector3 escapePosition = transform.position + awayDirection * safeDistance;
            agent.SetDestination(escapePosition);

            if (distanceToPlayer > safeDistance) agent.isStopped = false; // Se il giocatore è abbastanza vicino, muove la pecora
            else agent.isStopped = true; // Se il giocatore è abbastanza lontano, ferma la pecora
        }
        else agent.isStopped = true; // Se il giocatore è abbastanza lontano, ferma la pecora
    }

    public void SheepInArea()
    {
        if (targetEndTask != null)
        {
            // BooleanAccessor.istance.SetBoolOnDialogueE("acquaDone"); // Cambiare il nome della variabile a pecoraIsInArea, ad es
            // Qui settare che la quest è completata
            // Aggiungere SFX di "beee" della pecora appena entra nell'area
            // Al max da qui potresti far partire un breve dialogo (dovrebbe entrare solo una volta in questa funzione -> appena la pecora entra nell'area di trigger)
            hasEnteredTargetArea = true;
            agent.SetDestination(targetEndTask.position);
            agent.isStopped = false;
        }
    }
}
