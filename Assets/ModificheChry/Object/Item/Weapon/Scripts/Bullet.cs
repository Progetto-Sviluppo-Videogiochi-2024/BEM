using DialogueEditor;
using UnityEngine;

public class Bullet : MonoBehaviour // TODO: adattare questo script per il progetto quando spara villain o oggetti (se necessario)
{
    [Header("Bullet Settings")]
    #region Bullet Settings
    [SerializeField] float timeToDestroy;
    [HideInInspector] public Vector3 direction;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    [HideInInspector] public WeaponManager weapon;
    #endregion

    private void Start()
    {
        Destroy(this.gameObject, timeToDestroy);
    }

    public void Hit(RaycastHit hit)
    {
        GameObject hitObject = hit.transform.gameObject;
        Debug.Log("Bullet hit: " + hitObject.name);

        var convManager = ConversationManager.Instance;
        if (hitObject.CompareTag("Shootable") && hitObject.name.Contains("CocaCola") && convManager.GetBool("cocacola"))
        {
            PlayerPrefs.SetInt("CocaCola", PlayerPrefs.GetInt("CocaCola") + 1);
            PlayerPrefs.Save();
        }

        if (hitObject.transform.root.gameObject.GetComponent<EnemyHealth>())
        {
            EnemyHealth enemyHealth = hitObject.transform.root.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(weapon.damage);

            if (enemyHealth.maxHealth <= 0 && !enemyHealth.isDead)
            {
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                rb.AddForce(direction * weapon.enemykickBackForce, ForceMode.Impulse);
                enemyHealth.isDead = true;
            }
        }
        Destroy(this.gameObject);
    }
}
