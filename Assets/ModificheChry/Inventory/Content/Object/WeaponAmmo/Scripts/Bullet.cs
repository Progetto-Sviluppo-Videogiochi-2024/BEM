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

    void Start() => Destroy(this.gameObject, timeToDestroy);

    public void Hit(RaycastHit hit)
    {
        GameObject hitObject = hit.transform.gameObject;
        print("Bullet.Hit: " + hitObject.name);

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

        var mutant = hitObject.transform.GetComponentInParent<IEnemyStatus>();
        if (mutant != null)
        {
            var partSystem = hitObject.transform.GetChild(0).gameObject.GetComponent<TriggerParticles>();
            partSystem?.PlayParticles();
            mutant.TakeDamage(weapon.damage);
            if (!mutant.IsEnemyAlive())
            {
                Rigidbody rb = hitObject.GetComponent<Rigidbody>();
                rb?.AddForce(direction * weapon.enemykickBackForce, ForceMode.Impulse);
            }
        }

        Destroy(this.gameObject);
    }
}
