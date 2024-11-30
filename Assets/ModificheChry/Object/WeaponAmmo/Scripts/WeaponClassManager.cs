using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static Item;
using static Weapon;
using static RigSwitcher;

public class WeaponClassManager : MonoBehaviour
{
   // [Header("Animation Rigging")]
   // #region Animation Rigging
   // [SerializeField] TwoBoneIKConstraint leftHandIK;
   // #endregion

    [Header("Equip Weapons")]
    #region Switching-Equip Weapons
    public RigSwitcher CambiaRig;
    int currentWeaponIndex = -1;
    #endregion
    [HideInInspector] public GameObject currentWeapon;
    public Transform weaponHolder;

    [Header("References")]
    #region References
    public Transform recoilFollowPosition;
    [HideInInspector] public Animator animator;
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    [HideInInspector] public ActionStateManager actions;
    public List<WeaponManager> weaponsEquipable;
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
        actions = GetComponent<ActionStateManager>();
        CambiaRig =  GetComponentInChildren<RigSwitcher>();
    }

    private void Update()
    {
        for (int i = 0; i < weaponsEquipable.Count; i++)
        {
            // Se il tasto premuto è tra 1 e 3 e l'arma associata a quello slot è impostata
            if (Input.GetKeyDown(KeyCode.Alpha1 + i) && KeyCode.Alpha1 + i <= KeyCode.Alpha3 && weaponsEquipable[i] != null)
            {
                TakeParametersAnimation(i);
                return;
            }
        }
    }

    private void TakeParametersAnimation(int index)
    {
        string trigger;
        string boolWeapon;

        var weapon = InventoryManager.instance.weaponsEquipable[index] as Weapon;

        if (weapon.weaponType == WeaponType.Melee) // Se è un'arma da mischia o bianca
        {
            trigger = "equipMelee";
            boolWeapon = "hasCutWeapon";
        }
        else // Se è un'arma da fuoco
        {
            trigger = "equipRanged";
            boolWeapon = "hasFireWeapon";
        }
        ActiveAnimationWeapon(weapon, boolWeapon, trigger, index);
    }

    private void ActiveAnimationWeapon(Weapon weapon, string boolAnim, string triggerAnim, int index)
    {
        animator.SetTrigger(triggerAnim);

        if (animator.GetBool(boolAnim) && weapon.weaponType == WeaponType.Ranged && animator.GetBool("aiming")) // Se sto mirando con l'arma da fuoco
        {
            animator.SetBool("aiming", false); // Disattiva la mira
        }

        StartCoroutine(WaitForEquipAnimation(boolAnim, animator.GetBool(boolAnim) ? "Rifle Put Away" : "Rifle Pull Out", index));
    }

    private IEnumerator WaitForEquipAnimation(string boolWeapon, string animation, int index)
    {
        // Aspetta che l'animazione di equipaggiamento sia finita
        yield return new WaitUntil(() => IsAnimationFinished("Action", animation, 0.10f));

        animator.SetBool(boolWeapon, !animator.GetBool(boolWeapon));
        EquipWeapon(index);
    }

    private bool IsAnimationFinished(string layer, string animation, float normalizedTime)
    {
        // Controlla se l'animazione corrente "animation" nel layer "layer" sia almeno al "normalizedTime"
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layer));
        return animStateInfo.IsName(animation) && animStateInfo.normalizedTime >= normalizedTime;
    }

    void EquipWeapon(int index)
    {
        if (AlreadyEquippedRemoveHand(index)) return; // Se l'arma è già equipaggiata, la toglie dalla mano
        else if (index >= 0 && index < weaponsEquipable.Count)
        {
            // Controlla se un'arma è già equipaggiata
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);  // Rimuove l'arma attualmente equipaggiata
            }

            // Imposta l'arma corrente
            currentWeapon = weaponsEquipable[index].gameObject;
            currentWeapon.name = weaponsEquipable[index].gameObject.name;
            
            // Attiva e posiziona la nuova arma
            currentWeapon.SetActive(true);
            currentWeapon.transform.SetParent(weaponHolder);
            //currentWeapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity); serve ???
            CambiaRig.SwitchWeapon(currentWeapon);
            currentWeaponIndex = index; // Aggiorna l'indice dell'arma corrente

            // Rimuovi eventuali componenti non necessari dall'arma
            if (currentWeapon.TryGetComponent<ItemPickup>(out var itemPickup))
            {
                Destroy(itemPickup);  // Rimuove il componente ItemPickup se presente
            }

            SetCurrentWeapon(weaponsEquipable[index]);

            // Imposta le trasformazioni dell'arma equipaggiata
        }
    }

    public void RemoveEquipable(Item item)
    {
        if (item.tagType == ItemTagType.Weapon)
        {
            GameObject itemPrefab = item.prefab;
            if (weaponsEquipable.Contains(itemPrefab.GetComponent<WeaponManager>()))
            {
                RemoveWeaponHand();
                animator.SetBool("hasFireWeapon", false);
                animator.SetBool("hasCutWeapon", false);
                weaponsEquipable.Remove(itemPrefab.GetComponent<WeaponManager>());
            }
        }
    }

    private bool AlreadyEquippedRemoveHand(int index)
    {
        if (index == currentWeaponIndex && currentWeapon != null)
        {
            RemoveWeaponHand();
            return true;
        }
        return false; // Se non era equipaggiato, ritorna false
    }

    private void RemoveWeaponHand()
    {
        if (currentWeapon == null) return;
        currentWeapon.SetActive(false);
        currentWeapon.transform.SetParent(null);
        currentWeapon = null;
        currentWeaponIndex = -1;
        CambiaRig.RemoveCurrentRig();
    }

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        if (actions == null) actions = GetComponent<ActionStateManager>();
        /*leftHandIK.data.target = weapon.leftHandTarget;
        leftHandIK.data.hint = weapon.leftHandHint;*/
        actions.SetWeapon(weapon);
    }

    public void SwitchWeapon(float direction)
    {
        if (weaponsEquipable.Count <= 1) return; 

        weaponsEquipable[currentWeaponIndex].gameObject.SetActive(false);
        if (direction < 0)
        {
            if (currentWeaponIndex == 0) currentWeaponIndex = weaponsEquipable.Count - 1;
            else currentWeaponIndex--;
        }
        else
        {
            if (currentWeaponIndex == weaponsEquipable.Count - 1) currentWeaponIndex = 0;
            else currentWeaponIndex++;
        }
        weaponsEquipable[currentWeaponIndex].gameObject.SetActive(true);
    }

    public void WeaponPutAway() => actions.SwitchState(actions.defaultState); // Invocata dall'animazione Rifle Put Away

    public void WeaponPulledOut() => actions.SwitchState(actions.defaultState); // Invocata dall'animazione Rifle Pull Out
}
