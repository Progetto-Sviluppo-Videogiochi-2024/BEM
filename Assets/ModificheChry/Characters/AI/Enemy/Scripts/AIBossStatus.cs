
using UnityEngine;

public class AIBossStatus : MonoBehaviour, IEnemyStatus
{
    [Header("Settings")]
    #region Enemy Settings
    [SerializeField] float health; // Salute dell'IA
    bool isDead; // Flag per controllare se l'IA è morta
    public float Health { get => health; set => health = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    #endregion

    [Header("References")]
    #region References
    AIBossAgent bossAgent; // Riferimento alla macchina a stati
    #endregion

    void Start()
    {
        bossAgent = GetComponent<AIBossAgent>();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) { Debug.LogError($"Damage {damage} must be > than 0"); return; }
        if (!IsEnemyAlive()) return;

        Health -= damage;
        if (!IsEnemyAlive()) { Health = 0; bossAgent.stateMachine.ChangeState(AIStateId.Death); }
    }

    public bool IsEnemyAlive() => Health > 0;
}
