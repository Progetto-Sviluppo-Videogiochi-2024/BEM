using UnityEngine;

public class AIStatus : MonoBehaviour
{
    [Header("Settings")]
    #region Enemy Settings
    [SerializeField] public float health;
    [HideInInspector] public bool isDead;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    RagdollManager ragdoll;
    #endregion

    private void Start()
    {
        ragdoll = GetComponent<RagdollManager>();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0) { Debug.LogError($"Damage {damage} must be > than 0"); return; }
        if (!IsEnemyAlive()) return;

        health -= damage;
        if (!IsEnemyAlive()) { health = 0; Death(); }
        print("Enemy Health: " + health);
    }

    private void Death()
    {
        ragdoll.TriggerRagdoll(); // TODO: ragdoll o animazione ?
        print("Enemy is dead");
    }

    public bool IsEnemyAlive() => health > 0;
}
