using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Pickup")]
    #region Pickup
    private Item item; // Oggetto da raccogliere
    private bool isPlayerInRange = false; // Per sapere se il giocatore è vicino all'oggetto
    private bool isItemAdded = false; // Flag per assicurarsi che l'oggetto venga aggiunto solo una volta
    #endregion

    [Header("References")]
    #region References
    private Animator animator; // Riferimento all'Animator del giocatore
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    private OpenInventory openMenuScript; // Riferimento allo script OpenMenu
    #endregion

    private void Start()
    {
        item = GetComponent<ItemController>().item;
        openMenuScript = FindObjectOfType<Player>().gameObject.GetComponent<OpenInventory>();
        animator = FindObjectOfType<Player>().gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (isPlayerInRange && item.isPickUp) // Se è vicino a un oggetto raccoglibile
        {
            // if (Input.GetMouseButtonDown(0) && CheckClickMouseItem()) PickUp(); else // TODO: commentato perché bisogna capire se lasciamo il click
            if (!animator.GetBool("pickingUp") && Input.GetKeyDown(KeyCode.Space)) PickUp();
            else if (animator.GetBool("pickingUp") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))) CancelPickup();
        }
    }

    // private bool CheckClickMouseItem()
    // {
    //     // Raycast per determinare se il clic è effettivamente sull'oggetto
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         if (hit.transform == transform && !animator.GetBool("pickingUp")) return true;
    //     }
    //     return false;
    // }

    private void PickUp()
    {
        // Animo il giocatore per raccogliere l'oggetto
        animator.SetBool("pickingUp", true);
        if (openMenuScript.isInventoryOpen || openMenuScript.itemInspectOpen != null)
        { openMenuScript.ToggleInventory(false); }
        StartCoroutine(WaitForEquipAnimation());
    }

    private void CancelPickup() => animator.SetBool("pickingUp", false);

    private IEnumerator WaitForEquipAnimation()
    {
        // Attendi che l'animazione di equipaggiamento sia finita
        yield return new WaitUntil(() => IsAnimationFinished("Action", "Taking Item", 0.3f));

        // Verifica se l'oggetto è già stato aggiunto
        if (!isItemAdded && animator.GetBool("pickingUp"))
        {
            if (item.tagType != Item.ItemTagType.Scene)
            {
                if (item.tagType == Item.ItemTagType.Weapon)
                {
                    item.prefab = Instantiate(gameObject);
                    item.prefab.SetActive(false);
                }
                InventoryManager.instance.Add(item);
                InventoryUIController.instance.ListItems(InventoryManager.instance.items);

            }
            FindObjectOfType<GestoreScena>().SetItemScene(item);

            isItemAdded = true;

            FindObjectOfType<Player>().GetComponent<ItemDetector>().RemoveItemDetection(gameObject);
            if (item.canDestroy) Destroy(gameObject);
        }

        animator.SetBool("pickingUp", false);
    }

    private bool IsAnimationFinished(string layer, string animation, float normalizedTime)
    {
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layer));
        return animStateInfo.IsName(animation) && animStateInfo.normalizedTime >= normalizedTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il giocatore entra nel raggio di raccolta
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Controlla se il giocatore esce dal raggio di raccolta
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
