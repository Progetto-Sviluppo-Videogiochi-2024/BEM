using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    #region Fire Rate
    public LayerMask layerMask; // LayerMask per il raggio di sparo
    private bool isEmptyAmmoSoundPlayed = false; // Flag per evitare spam della SFX
    [HideInInspector] public int bulletConsumed = 0; // Munizioni consumate
    private bool isValidate = false; // Flag per validare lo script
    private bool isInitialized = false; // Flag per inizializzare lo script una sola volta
    [SerializeField] float fireRate; // Tempo tra uno sparo e l'altro
    float fireRateTimer; // Timer per il tempo tra uno sparo e l'altro
    #endregion

    [Header("Bullet Properties")]
    #region Bullet Properties
    [SerializeField] private GameObject bulletPrefab; // Prefab del proiettile
    [HideInInspector] private Transform bulletSpawnPoint; // Punto di spawn del proiettile
    [SerializeField] float bulletVelocity; // Velocità del proiettile
    [SerializeField] int bulletsPerShot; // Numero di proiettili sparati per colpo
    [HideInInspector] public float damage; // Danno del proiettile
    #endregion

    [Header("Audio Properties")]
    #region Audio Properties
    public AudioClip fireSound; // Suono di sparo
    public AudioClip magInSound; // Suono di inserimento del caricatore
    public AudioClip magOutSound; // Suono di estrazione del caricatore
    public AudioClip releaseSlideSound; // Suono di rilascio del caricatore
    [HideInInspector] public AudioSource audioSource; // Sorgente audio
    #endregion

    [Header("Muzzle Flash Properties")]
    #region Muzzle Flash Properties
    //Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles; // Particelle del flash del proiettile
    //float lightIntensity;
    // const float lightReturnSpeed = 20;
    #endregion

    [Header("Enemy Properties")]
    #region Enemy Properties
    public float enemykickBackForce = 100;
    #endregion

    [Header("References Scripts")]
    #region References
    [HideInInspector] public WeaponAmmo ammo; // Riferimento allo script WeaponAmmo
    WeaponBloom bloom; // Riferimento allo script WeaponBloom
    AimStateManager aim; // Riferimento allo script AimStateManager
    ActionStateManager actions; // Riferimento allo script ActionStateManager
    WeaponRecoil recoil; // Riferimento allo script WeaponRecoil
    [HideInInspector] public WeaponClassManager weaponClassManager; // Riferimento allo script WeaponClassManager
    [HideInInspector] public Weapon weapon; // Riferimento allo script Weapon
    #endregion

    private void Awake()
    {
        weapon = GetComponent<ItemController>().item as Weapon;
        weaponClassManager = FindAnyObjectByType<WeaponClassManager>();
        recoil = GetComponent<WeaponRecoil>();
        recoil.recoilFollowPosition = weaponClassManager.recoilFollowPosition;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (!Validate()) return;
        isValidate = true;
        bulletConsumed = (GetComponent<ItemController>().item as Weapon).bulletConsumed;

        aim ??= weaponClassManager.GetComponent<AimStateManager>();
        actions ??= weaponClassManager.GetComponent<ActionStateManager>();
        bloom ??= GetComponent<WeaponBloom>();

        fireRateTimer = fireRate;
    }

    private void OnEnable()
    {
        if (isInitialized) return;
        bulletSpawnPoint = gameObject.transform.Find("BulletSpawnPoint");
        muzzleFlashParticles = bulletSpawnPoint.GetComponentInChildren<ParticleSystem>();

        damage = weapon.ammo.damageAmmo;
        ammo = GetComponent<WeaponAmmo>();
        ammo.data = weapon.ammo;
        ammo.isLoadingSlot = (GetComponent<ItemController>().item as Weapon).isLoadingSlot;
        ammo.Init();
        isInitialized = true;
    }

    void OnBecameVisible()
    {
        if (!isValidate && Validate()) { Start(); OnEnable(); }
    }

    void Update()
    {
        if (!isValidate) return;

        if (ShouldFire()) Fire();
        if (!Input.GetKey(KeyCode.Space)) isEmptyAmmoSoundPlayed = false; // Resetta il flag quando si smette di premere il tasto di sparo
    }

    private bool Validate() => GetComponent<ItemController>() != null && transform.root.GetChild(0).CompareTag("Player"); // Se l'oggetto ha un ItemController e il padre ha il tag "Player"

    public void SetIdle() => transform.SetLocalPositionAndRotation(weapon.IdlePosition, weapon.IdleRotation);

    public void SetAim()
    {
        transform.SetLocalPositionAndRotation(weapon.AimPosition, weapon.AimRotation);
        transform.localScale = weapon.Scale;
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (aim.currentState == aim.rifleIdleState) return false; // Se sta in idle con l'arma, non sparare
        if (fireRateTimer < fireRate) return false; // Se il timer non è ancora scaduto, non sparare
        if (ammo.currentAmmo == 0 || (ammo.currentAmmo == 0 && ammo.extraAmmo == 0))
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.Space)) && !isEmptyAmmoSoundPlayed)
            {
                isEmptyAmmoSoundPlayed = true;
                audioSource.PlayOneShot(actions.emptyAmmoSound);
            }
            return false;
        }
        if (actions.currentState == actions.reloadState) return false; // Se si sta ricaricando, non sparare
        if (actions.currentState == actions.swapState) return false; // Se si sta cambiando arma, non sparare
        if (EventSystem.current.IsPointerOverGameObject()) return false; // Se il mouse è sopra un UI, non sparare
        if (IsClickingOnInteractiveObject()) return false; // Se si sta cliccando su un oggetto interattivo (raccoglibile, ecc.), non sparare
        if (weapon.semiAuto && Input.GetKeyDown(KeyCode.Space)) return true; // Se l'arma è semi automatica e si preme il tasto sinistro del mouse, sparare
        if (!weapon.semiAuto && Input.GetKey(KeyCode.Space)) return true; // Se l'arma è automatica e si tiene premuto il tasto sinistro del mouse, sparare
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;
        bulletSpawnPoint.LookAt(aim.aimPos);
        bulletSpawnPoint.localEulerAngles = bloom.BloomAngle(bulletSpawnPoint);
        audioSource.PlayOneShot(fireSound);
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

        bulletConsumed++;
        if (bulletConsumed >= ammo.clipSize && ammo.extraAmmo >= 0 && ammo.currentAmmo >= 0)
        {
            InventoryManager.instance.Remove(weapon.ammo, true);
            bulletConsumed = 0; // Reset del contatore dopo aver rimosso il caricatore
            print($"Caricatore rimosso: {ammo.extraAmmo} + {ammo.currentAmmo} munizioni rimanenti.\nCaricatori restanti: {ammo.extraAmmo / ammo.clipSize}");
        }
    }

    void TriggerMuzzleFlash() => muzzleFlashParticles.Play();

    private bool IsClickingOnInteractiveObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) return hit.transform.GetComponent<ItemPickup>() != null;
        return false; // Se non c'è nulla di interattivo, ritorna false
    }
}
