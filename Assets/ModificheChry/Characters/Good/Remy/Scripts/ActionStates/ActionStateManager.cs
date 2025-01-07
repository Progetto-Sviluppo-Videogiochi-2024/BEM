using UnityEngine;

public class ActionStateManager : MonoBehaviour
{
    [Header("States")]
    #region States
    [HideInInspector] public bool isSwapping = false;
    public DefaultState defaultState = new();
    public ReloadState reloadState = new();
    public SwapState swapState = new();
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Animator animator;
    [HideInInspector] AudioSource audioSource;
    public AudioClip emptyAmmoSound; // Suono quando spara senza munizioni
    #endregion

    [Header("References Scripts")]
    #region Weapon
    [HideInInspector] public ActionBaseState currentState;
    [HideInInspector] public WeaponManager currentWeapon;
    [HideInInspector] public WeaponAmmo weaponAmmo;
    [HideInInspector] public WeaponClassManager weaponClassManager;
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        weaponClassManager = GetComponent<WeaponClassManager>();
        SwitchState(defaultState);
    }

    void Update() => currentState.UpdateState(this);

    public void SwitchState(ActionBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void WeaponReloaded() // 4 funzione invocata dall'animazione reloading
    {
        weaponAmmo.Reload();
        SwitchState(defaultState);
    }

    public void MagOut() => audioSource.PlayOneShot(currentWeapon.weapon.magOutSound); // 1 funzione invocata dall'animazione reloading

    public void MagIn() => audioSource.PlayOneShot(currentWeapon.weapon.magInSound); // 2 funzione invocata dall'animazione reloading

    public void ReleaseSlide() => audioSource.PlayOneShot(currentWeapon.weapon.releaseSlideSound); // 3 funzione invocata dall'animazione reloading

    public void SetWeapon(WeaponManager weapon)
    {
        currentWeapon = weapon;
        audioSource = weapon.audioSource;
        weaponAmmo = weapon.ammo;
    }

    public void SetAmmo(Ammo ammo)
    {
        // Assicurati che maxAmmo non sia zero per evitare errori di calcolo => non ho raccolto munizioni per quell'arma
        if (ammo.maxAmmo <= 0)
        {
            weaponAmmo.extraAmmo = 0;
            weaponAmmo.currentAmmo = 0;
            print($"{currentWeapon.weapon.name} non ha munizioni: extraAmmo = {weaponAmmo.extraAmmo}, currentAmmo = {weaponAmmo.currentAmmo}");
            return;
        }

        weaponAmmo.currentAmmo = Mathf.Clamp(ammo.nAmmo, 0, ammo.maxAmmo); // currentAmmo \in [0, maxAmmo]
        weaponAmmo.extraAmmo = Mathf.Max(ammo.maxAmmo - weaponAmmo.currentAmmo, 0); // extraAmmo \in [0, maxAmmo - currentAmmo]
        print($"{currentWeapon.weapon.name} ha {weaponAmmo.currentAmmo} munizioni e {weaponAmmo.extraAmmo} munizioni extra");
    }
}
