
using System.Collections;
using UnityEngine;

public class AIBossStatus : MonoBehaviour, IEnemyStatus
{
    [Header("Settings")]
    #region Enemy Settings
    [SerializeField] float health; // Salute dell'IA
    bool isDead; // Flag per controllare se l'IA è morta
    public float Health { get => health; set => health = value; } // Proprietà per accedere alla salute dell'IA
    public bool IsDead { get => isDead; set => isDead = value; } // Proprietà per accedere al flag isDead
    public float stunnedHP; // HP max per lo stordimento
    public bool isStunned = false; // Flag per controllare se l'IA è stordita, avviene una sola volta
    #endregion

    [Header("References")]
    #region References
    AIBossAgent bossAgent; // Riferimento alla macchina a stati
    public ZonaBoss zonaBoss; // Riferimento alla zona boss
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
        if (Health <= stunnedHP && Health > 0 && !isStunned) // Stordimento
        {
            isStunned = true;
            bossAgent.animator.SetBool("isStunned", true);
            bossAgent.animator.SetTrigger("death");
            bossAgent.stateMachine.ChangeState(AIStateId.Death);
            zonaBoss.BossRecovery();
        }
        else if (!IsEnemyAlive())
        {
            Health = 0;
            bossAgent.animator.SetTrigger("death");
            bossAgent.stateMachine.ChangeState(AIStateId.Death);
        }
    }

    public bool IsEnemyAlive() => Health > 0;
}
