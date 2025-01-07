using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;

public class WeaponClassManager : MonoBehaviour
{
    [Header("Equip Weapons")]
    #region Switching-Equip Weapons
    [HideInInspector] public int equipWeaponIndex; // Indice temporaneo dell'arma da equipaggiare // Per evitare che lo scroll non mi tolga l'arma dalla mano in AlreadyEquippedRemoveHand()
    [HideInInspector] public int currentWeaponIndex = -1; // Indice dell'arma corrente
    [HideInInspector] public GameObject currentWeapon; // Arma corrente
    public Transform weaponHolder; // Posizione dell'arma equipaggiata
    [HideInInspector] public bool isWeaponEquipFinished = true; // Verifica se l'animazione di equipaggiamento è finita
    #endregion

    [Header("References")]
    #region References
    public Transform recoilFollowPosition; // Posizione di rinculo dell'arma
    [HideInInspector] public Animator animator; // Animator del personaggio
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    [HideInInspector] public ActionStateManager actions; // Riferimento allo script ActionStateManager
    public List<WeaponManager> weaponsEquipable; // Lista delle armi equipaggiabili
    AimStateManager aim; // Riferimento allo script AimStateManager
    #endregion

    void Start()
    {
        animator = GetComponent<Animator>();
        actions = GetComponent<ActionStateManager>();
        aim = GetComponent<AimStateManager>();
    }

    void Update()
    {
        if (!isWeaponEquipFinished) return; // Se l'animazione di equipaggiamento non è finita, non fare nulla

        for (int i = 0; i < weaponsEquipable.Count; i++)
        {
            // Se il tasto premuto è tra 1 e 3 e l'arma associata a quello slot è impostata
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && KeyCode.Alpha1 + i <= KeyCode.Alpha4 && weaponsEquipable[i] != null)
            {
                if (!AlreadyEquippedOrNull(i)) { actions.SwitchState(actions.swapState); SwitchWeapon(i); return; } // Se l'arma non è già equipaggiata, la equipaggia
                // Else if arma corrente == (null | arma da equipaggiare) do:
                isWeaponEquipFinished = false;
                equipWeaponIndex = i;
                ToggleAnimationWeapon(i);
                return;
            }
        }
    }

    public void ToggleAnimationWeapon(int index)
    {
        // var weapon = InventoryManager.instance.weaponsEquipable[index] as Weapon;
        string triggerAnim = "equipRanged";
        string boolAnim = "hasFireWeapon";

        animator.SetTrigger(triggerAnim);
        if (animator.GetBool(boolAnim) && animator.GetBool("aiming")) // Se sto mirando con l'arma da fuoco
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
        if (AlreadyEquippedOrNull(index) && currentWeapon != null) RemoveWeaponHand(); // Se l'arma è già equipaggiata, la toglie dalla mano
        else if (index >= 0 && index < weaponsEquipable.Count) // Se non è equipaggiata e l'indice è valido, la equipaggia
        {
            // Controlla se un'arma è già equipaggiata
            if (currentWeapon != null) { currentWeapon.SetActive(false); currentWeapon.transform.SetParent(null); }

            // Imposta l'arma corrente
            currentWeapon = weaponsEquipable[index].gameObject;
            currentWeapon.name = weaponsEquipable[index].gameObject.name;

            // Attiva e posiziona la nuova arma
            currentWeapon.SetActive(true);
            currentWeapon.transform.SetParent(weaponHolder);
            weaponsEquipable[index].SetIdle();

            aim.SwitchState(aim.rifleIdleState);
            currentWeaponIndex = index; // Aggiorna l'indice dell'arma corrente

            // Rimuovi eventuali componenti non necessari dall'arma
            if (currentWeapon.TryGetComponent<ItemPickup>(out var itemPickup)) Destroy(itemPickup);  // Rimuove il componente ItemPickup se presente

            SetCurrentWeapon(weaponsEquipable[index]);
            SetCurrentAmmo(weaponsEquipable[index]);
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
                weaponsEquipable.Remove(weaponManager);
            }
        }
    }

    private bool AlreadyEquippedOrNull(int index) => currentWeaponIndex == -1 || index == currentWeaponIndex;

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

    public void SetCurrentAmmo(WeaponManager weapon)
    {
        actions ??= GetComponent<ActionStateManager>();
        actions.SetAmmo(weapon.weapon.ammo);
    }

    public void SwitchWeapon(float direction)
    {
        if (weaponsEquipable.Count <= 1) return;
        actions.isSwapping = true;
        weaponsEquipable[currentWeaponIndex].gameObject.SetActive(false);

        equipWeaponIndex = currentWeaponIndex;
        if (direction > 0) equipWeaponIndex = (equipWeaponIndex + 1) % weaponsEquipable.Count;
        else equipWeaponIndex = (equipWeaponIndex - 1 + weaponsEquipable.Count) % weaponsEquipable.Count;
        equipWeaponIndex = Mathf.Clamp(equipWeaponIndex, 0, weaponsEquipable.Count - 1);
        // EquipWeapon(equipWeaponIndex);
    }

    public void SwapWeaponPutAway() { } // Invocata dall'animazione Rifle Put Away

    public void SwapWeaponPulledOut() // Invocata dall'animazione Rifle Pull Out
    {
        if (!actions.isSwapping) return;
        EquipWeapon(equipWeaponIndex);
        actions.SwitchState(actions.defaultState);
        actions.isSwapping = false;
    }
}
