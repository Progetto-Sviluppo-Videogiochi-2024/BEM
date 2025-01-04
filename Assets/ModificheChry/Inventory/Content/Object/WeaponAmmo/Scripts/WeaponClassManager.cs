using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;
using static Weapon;

public class WeaponClassManager : MonoBehaviour
{
    [Header("Equip Weapons")]
    #region Switching-Equip Weapons
    [HideInInspector] public int currentWeaponIndex = -1;
    [HideInInspector] public GameObject currentWeapon;
    public Transform weaponHolder;
    private bool isWeaponEquipFinished = true;
    #endregion

    [Header("References")]
    #region References
    public Transform recoilFollowPosition;
    [HideInInspector] public Animator animator;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    [HideInInspector] public ActionStateManager actions;
    public List<WeaponManager> weaponsEquipable;
    AimStateManager aim;
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
        actions = GetComponent<ActionStateManager>();
        aim = GetComponent<AimStateManager>();
    }

    private void Update()
    {
        for (int i = 0; i < weaponsEquipable.Count; i++)
        {
            // Se il tasto premuto è tra 1 e 3 e l'arma associata a quello slot è impostata
            if (isWeaponEquipFinished && Input.GetKeyDown(KeyCode.Alpha1 + i) && KeyCode.Alpha1 + i <= KeyCode.Alpha3 && weaponsEquipable[i] != null)
            {
                isWeaponEquipFinished = false;
                ActiveAnimationWeapon(i);
                return;
            }
        }
    }

    public void ActiveAnimationWeapon(int index)
    {
        var weapon = InventoryManager.instance.weaponsEquipable[index] as Weapon;
        string triggerAnim = "equipRanged";
        string boolAnim = "hasFireWeapon";

        animator.SetTrigger(triggerAnim);
        if (animator.GetBool(boolAnim) && weapon.weaponType == WeaponType.Ranged && animator.GetBool("aiming")) // Se sto mirando con l'arma da fuoco
        {
            animator.SetBool("aiming", false); // Disattiva la mira
        }
        StartCoroutine(WaitForEquipAnimation(boolAnim, index));
    }

    private IEnumerator WaitForEquipAnimation(string boolWeapon, int index)
    {
        // Aspetta che l'animazione di equipaggiamento sia finita
        yield return new WaitUntil(() => IsAnimationFinished("Action", animator.GetBool(boolWeapon) ? "Rifle Put Away" : "Rifle Pull Out", 0.10f));

        isWeaponEquipFinished = true;
        animator.SetBool(boolWeapon, !animator.GetBool(boolWeapon));
        EquipWeapon(index);
    }

    private bool IsAnimationFinished(string layer, string animation, float normalizedTime)
    {
        // Controlla se l'animazione corrente "animation" nel layer "layer" sia almeno al "normalizedTime"
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layer));
        return animStateInfo.IsName(animation) && animStateInfo.normalizedTime >= normalizedTime;
    }

    public void EquipWeapon(int index)
    {
        if (AlreadyEquippedRemoveHand(index)) return; // Se l'arma è già equipaggiata, la toglie dalla mano
        else if (index >= 0 && index < weaponsEquipable.Count)
        {
            // Controlla se un'arma è già equipaggiata
            if (currentWeapon != null) Destroy(currentWeapon);  // Rimuove l'arma attualmente equipaggiata

            // Imposta l'arma corrente
            currentWeapon = weaponsEquipable[index].gameObject;
            currentWeapon.name = weaponsEquipable[index].gameObject.name;

            // Attiva e posiziona la nuova arma
            currentWeapon.SetActive(true);
            currentWeapon.transform.SetParent(weaponHolder);
            weaponsEquipable[index].SwitchWeapon();

            aim.SwitchState(aim.rifleIdleState);
            currentWeaponIndex = index; // Aggiorna l'indice dell'arma corrente

            // Rimuovi eventuali componenti non necessari dall'arma
            if (currentWeapon.TryGetComponent<ItemPickup>(out var itemPickup)) Destroy(itemPickup);  // Rimuove il componente ItemPickup se presente

            SetCurrentWeapon(weaponsEquipable[index]);
        }
    }

    public void RemoveEquipable(Item item)
    {
        if (item.tagType == ItemTagType.Weapon)
        {
            WeaponManager weaponManager = (item as Weapon).prefab.GetComponent<WeaponManager>();
            if (weaponsEquipable.Contains(weaponManager))
            {
                RemoveWeaponHand();
                animator.SetBool("hasFireWeapon", false);
                animator.SetBool("hasCutWeapon", false);
                weaponsEquipable.Remove(weaponManager);
            }
        }
    }

    private bool AlreadyEquippedRemoveHand(int index)
    {
        if (index == currentWeaponIndex && currentWeapon != null) { RemoveWeaponHand(); return true; } // Se era già equipaggiato, lo toglie dalla mano
        return false; // Se non era equipaggiato, ritorna false
    }

    private void RemoveWeaponHand()
    {
        if (currentWeapon == null) return;

        currentWeapon.SetActive(false);
        currentWeapon.transform.SetParent(null);
        currentWeapon = null;
        currentWeaponIndex = -1;
        aim.currentState = null;
    }

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        actions ??= GetComponent<ActionStateManager>();
        actions.SetWeapon(weapon);
    }

    public void SwitchWeapon(float direction)
    {
        if (weaponsEquipable.Count <= 1) return;

        weaponsEquipable[currentWeaponIndex].gameObject.SetActive(false);
        if (direction < 0) currentWeaponIndex = currentWeaponIndex == 0 ? weaponsEquipable.Count - 1 : currentWeaponIndex - 1;
        else currentWeaponIndex = currentWeaponIndex == weaponsEquipable.Count - 1 ? 0 : currentWeaponIndex + 1;
        weaponsEquipable[currentWeaponIndex].gameObject.SetActive(true);
    }

    public void WeaponPutAway() => actions.SwitchState(actions.defaultState); // Invocata dall'animazione Rifle Put Away

    public void WeaponPulledOut() => actions.SwitchState(actions.defaultState); // Invocata dall'animazione Rifle Pull Out
}
