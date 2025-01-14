using UnityEngine;

public class AIDetection : MonoBehaviour
{
    [Header("Settings")]
    #region Detection Settings
    [SerializeField] float lookDistance = 30f; // Distanza di vista
    [SerializeField] float fov = 120f; // Campo visivo
    private bool isPlayerHeard = false; // Stato del rilevamento acustico
    [SerializeField] LayerMask layerMask; // Maschera per il rilevamento
    #endregion

    [Header("References")]
    #region References
    [SerializeField] Transform enemyEyes;
    Transform playerHead;
    private Transform playerTransform;
    private Player player;
    GestoreScena gestoreScena;
    [HideInInspector] AIStatus aIStatus;
    #endregion

    void Start()
    {
        playerTransform = GetComponent<AIAgent>().player.transform;
        player = playerTransform.GetComponent<Player>();
        aIStatus = GetComponent<AIStatus>();
        gestoreScena = FindObjectOfType<GestoreScena>();
        playerHead = gestoreScena?.playerHead;
    }

    private void FixedUpdate()
    {
        // Se il player è visto o sentito dal mutante (vivo)
        player.hasEnemyDetectedPlayer = aIStatus.IsEnemyAlive() && (IsPlayerSeen() || isPlayerHeard);
    }

    public bool IsPlayerSeen()
    {
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance) return false;

        Vector3 directionToPlayer = (playerHead.position - enemyEyes.position).normalized;
        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, directionToPlayer);
        if (angleToPlayer > (fov / 2)) return false;

        enemyEyes.LookAt(playerHead.position);

        if (Physics.Raycast(enemyEyes.position, enemyEyes.forward, out RaycastHit hit, lookDistance, layerMask))
        {
            if (hit.transform.name == playerTransform.name)
            {
                Debug.DrawLine(enemyEyes.position, hit.point, Color.green);
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerHeard = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerHeard = false;
    }
}
