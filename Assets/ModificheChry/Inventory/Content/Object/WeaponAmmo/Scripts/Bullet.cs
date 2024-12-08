using UnityEngine;

public class Bullet : MonoBehaviour
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
        print("Bullet hit: " + hitObject.name);

        var booleanAccessor = BooleanAccessor.istance;
        if (hitObject.CompareTag("Shootable") && hitObject.name.Contains("Shootable") && booleanAccessor.GetBoolFromThis("cocaCola"))
        {
            if (hit.collider.TryGetComponent<Bottle>(out var bottle))
            {
                bottle.Explode(hitObject);
                PlayerPrefs.SetInt("nTargetHit", PlayerPrefs.GetInt("nTargetHit") + 1);
                PlayerPrefs.Save();
            }
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
