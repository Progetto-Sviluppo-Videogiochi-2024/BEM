using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Settings")]
    #region Enemy Settings
    [SerializeField] public float maxHealth;
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
        if (maxHealth <= 0) return;

        maxHealth -= damage;
        if (maxHealth <= 0) { maxHealth = 0; EnemyDeath(); }
        Debug.Log("Enemy Health: " + maxHealth);
    }

    private void EnemyDeath()
    {
        ragdoll.TriggerRagdoll();
        Debug.Log("Enemy is dead");
    }
}
