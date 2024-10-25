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

    [Header("Animation Rigging")]
    #region Animation Rigging
    public MultiAimConstraint rightHandAim;
    public TwoBoneIKConstraint leftHandIK;
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

    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    public void WeaponReloaded() // 4 funzione invocata dall'animazione reloading
    {
        weaponAmmo.Reload();
        rightHandAim.weight = 1;
        leftHandIK.weight = 1;
        SwitchState(defaultState);
    }

    public void MagOut() => audioSource.PlayOneShot(weaponAmmo.magOutSound); // 1 funzione invocata dall'animazione reloading

    public void MagIn() => audioSource.PlayOneShot(weaponAmmo.magInSound); // 2 funzione invocata dall'animazione reloading

    public void ReleaseSlide() => audioSource.PlayOneShot(weaponAmmo.releaseSlideSound); // 3 funzione invocata dall'animazione reloading

    public void SetWeapon(WeaponManager weapon)
    {
        currentWeapon = weapon;
        audioSource = weapon.audioSource;
        weaponAmmo = weapon.ammo;
    }
}
