using UnityEngine;

public class AIStatus : MonoBehaviour
{
    [Header("Settings")]
    #region Enemy Settings
    [SerializeField] public float health;
    [HideInInspector] public bool isDead;
    #endregion

    [Header("References")]
    #region References
    AIAgent aIAgent;
    #endregion

    private void Start()
    {
        aIAgent = GetComponent<AIAgent>();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) { Debug.LogError($"Damage {damage} must be > than 0"); return; }
        if (!IsEnemyAlive()) return;

        health -= damage;
        if (!IsEnemyAlive()) { health = 0; aIAgent.stateMachine.ChangeState(AIStateId.Death); }
    }

    public bool IsEnemyAlive() => health > 0;
}
