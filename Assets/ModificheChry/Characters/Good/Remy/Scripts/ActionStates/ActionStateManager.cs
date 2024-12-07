using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    [Header("States")]
    #region States
    public DefaultState defaultState = new();
    public ReloadState reloadState = new();
    public SwapState swapState = new();
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public Animator animator;
    [HideInInspector] AudioSource audioSource;
    #endregion

    [Header("References Scripts")]
    #region Weapon
    [HideInInspector] public ActionBaseState currentState;
    [HideInInspector] public WeaponManager currentWeapon;
    [HideInInspector] public WeaponAmmo weaponAmmo;
    #endregion

    void Start()
    {
        SwitchState(defaultState);
        animator = GetComponent<Animator>();
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
}
