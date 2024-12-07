using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    #region Fire Rate
    [SerializeField] float fireRate;
    float fireRateTimer;
    public LayerMask layerMask;
    #endregion

    [Header("Bullet Properties")]
    #region Bullet Properties
    [HideInInspector] private GameObject bulletPrefab;
    [HideInInspector] private Transform bulletSpawnPoint;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletsPerShot;
    [HideInInspector] public float damage;
    #endregion

    [Header("Audio Properties")]
    #region Audio Properties
    [HideInInspector] public AudioSource audioSource;
    #endregion

    [Header("Muzzle Flash Properties")]
    #region Muzzle Flash Properties
    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    const float lightReturnSpeed = 20;
    #endregion

    [Header("Enemy Properties")]
    #region Enemy Properties
    public float enemykickBackForce = 100;
    #endregion

    // [Header("Animation Rigging Properties")]
    // #region Animation Rigging Properties
    // [HideInInspector] public Transform leftHandTarget;
    // [HideInInspector] public Transform leftHandHint;
    // #endregion

    [Header("References Scripts")]
    #region References
    [HideInInspector] public WeaponAmmo ammo;
    WeaponBloom bloom;
    AimStateManager aim;
    ActionStateManager actions;
    WeaponRecoil recoil;
    WeaponClassManager weaponClassManager;
    [HideInInspector] public Weapon weapon;
    #endregion

    private void Awake()
    {
        weapon = GetComponent<ItemController>().item as Weapon;
        weaponClassManager = FindAnyObjectByType<WeaponClassManager>();
        recoil = GetComponent<WeaponRecoil>();
        recoil.recoilFollowPosition = weaponClassManager.recoilFollowPosition;
    }

    void Start()
    {
        if (!Validate()) return;

        aim = GetComponentInParent<AimStateManager>();
        actions = GetComponentInParent<ActionStateManager>();
        bloom = GetComponent<WeaponBloom>();

        fireRateTimer = fireRate;
    }

    private void OnEnable()
    {
        bulletSpawnPoint = gameObject.transform.Find("BulletSpawnPoint");
        muzzleFlashParticles = bulletSpawnPoint.GetComponentInChildren<ParticleSystem>();
        muzzleFlashLight = bulletSpawnPoint.GetComponentInChildren<Light>();
        lightIntensity = 2;
        muzzleFlashLight.intensity = 0;

        damage = weapon.ammo.damageAmmo;
        bulletPrefab = weapon.ammo.prefab;

        ammo = GetComponent<WeaponAmmo>();
        ammo.data = weapon.ammo;

        audioSource = GetComponent<AudioSource>();

        // leftHandTarget = this.gameObject.transform.Find("LeftHandIK_target");
        // leftHandHint = this.gameObject.transform.Find("LeftHandIK_hint");
    }

    void Update()
    {
        if (!Validate()) return;

        if (ShouldFire()) Fire();
        muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
    }

    private bool Validate()
    {
        // Script valido sse è attaccato ad un oggetto con ItemController e il root è un Player
        if (GetComponent<ItemController>() == null || !transform.root.GetChild(0).CompareTag("Player")) return false;
        return true;
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (aim.currentState == aim.rifleIdleState) return false; // Se sta in idle con l'arma, non sparare
        if (fireRateTimer < fireRate) return false; // Se il timer non è ancora scaduto, non sparare
        if (ammo.currentAmmo == 0) return false; // Se le munizioni sono finite, non sparare
        if (actions.currentState == actions.reloadState) return false; // Se si sta ricaricando, non sparare
        if (actions.currentState == actions.swapState) return false; // Se si sta cambiando arma, non sparare
        if (EventSystem.current.IsPointerOverGameObject()) return false; // Se il mouse è sopra un UI, non sparare
        if (IsClickingOnInteractiveObject()) return false; // Se si sta cliccando su un oggetto interattivo (raccoglibile, ecc.), non sparare
        if (weapon.semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true; // Se l'arma è semi automatica e si preme il tasto sinistro del mouse, sparare
        if (!weapon.semiAuto && Input.GetKey(KeyCode.Mouse0)) return true; // Se l'arma è automatica e si tiene premuto il tasto sinistro del mouse, sparare
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;
        bulletSpawnPoint.LookAt(aim.aimPos);
        bulletSpawnPoint.localEulerAngles = bloom.BloomAngle(bulletSpawnPoint);
        audioSource.PlayOneShot(weapon.fireSound);
        recoil.TriggerRecoil();
        TriggerMuzzleFlash();
        ammo.currentAmmo--;
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * weapon.distance, Color.white, 10f); // Visualizza il raggio nella scena per debug
            if (Physics.Raycast(ray, out RaycastHit hit, weapon.distance, layerMask))
            {
                GameObject currentBullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                Vector3 direction = (hit.point - bulletSpawnPoint.position).normalized;
                Bullet bulletScript = currentBullet.GetComponent<Bullet>();
                bulletScript.weapon = this;
                bulletScript.direction = direction;
                Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
                rb.AddForce(direction * bulletVelocity, ForceMode.Impulse);
                bulletScript.Hit(hit);
            }
        }
    }

    void TriggerMuzzleFlash()
    {
        muzzleFlashParticles.Play();
        muzzleFlashLight.intensity = lightIntensity;
    }

    private bool IsClickingOnInteractiveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) return hit.transform.GetComponent<ItemPickup>() != null;
        return false; // Se non c'è nulla di interattivo, ritorna false
    }
}
