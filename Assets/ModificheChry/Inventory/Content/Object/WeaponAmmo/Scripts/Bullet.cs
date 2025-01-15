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

        // Per le bottiglie da colpire di Scena2
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

        // Per i mutanti di Scena3
        var mutant = hitObject.transform.GetComponentInParent<AIStatus>();
        if (mutant != null)
        {
            mutant.TakeDamage(weapon.damage);

            if (mutant.health <= 0 && !mutant.isDead)
            {
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                rb.AddForce(direction * weapon.enemykickBackForce, ForceMode.Impulse);
                mutant.isDead = true;
            }
        }

        Destroy(this.gameObject);
    }
}
