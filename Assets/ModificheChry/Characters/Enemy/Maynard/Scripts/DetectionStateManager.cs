using UnityEngine;

public class DetectionStateManager : MonoBehaviour
{
    [Header("Detection Settings")]
    #region Detection Settings
    [SerializeField] float lookDistance = 30f;
    [SerializeField] float fov = 120f;
    #endregion

    [Header("References")]
    #region References
    [SerializeField] Transform enemyEyes;
    Transform playerHead;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    GameManager gameManager;
    #endregion

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerHead = gameManager.playerHead;
    }

    private void FixedUpdate()
    {
        if (IsPlayerSeen() && IsEnemyAlive()) print("Player seen by " + transform.name);
        // else Debug.Log("Player not seen");
    }

    private bool IsEnemyAlive()
    {
        var enemy = enemyEyes.transform.root.gameObject.GetComponent<EnemyHealth>();
        return enemy.maxHealth > 0;
    }

    public bool IsPlayerSeen()
    {
        if (Vector3.Distance(enemyEyes.position, playerHead.position) > lookDistance) return false;

        Vector3 directionToPlayer = (playerHead.position - enemyEyes.position).normalized;
        float angleToPlayer = Vector3.Angle(enemyEyes.parent.forward, directionToPlayer);
        if (angleToPlayer > (fov / 2)) return false;

        enemyEyes.LookAt(playerHead.position);

        if (Physics.Raycast(enemyEyes.position, enemyEyes.forward, out RaycastHit hit, lookDistance))
        {
            if (hit.transform == null) return false;
            if (hit.transform.name == playerHead.root.name)
            {
                Debug.DrawLine(enemyEyes.position, hit.point, Color.green);
                return true;
            }
        }
        return false;
    }
}
