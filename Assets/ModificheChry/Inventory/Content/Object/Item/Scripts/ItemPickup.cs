using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemPickup : MonoBehaviour
{
    [Header("Pickup")]
    #region Pickup
    [HideInInspector] public Item item; // Oggetto da raccogliere
    private bool isPlayerInRange = false; // Per sapere se il giocatore è vicino all'oggetto
    private bool isItemAdded = false; // Flag per assicurarsi che l'oggetto venga aggiunto solo una volta
    private string itemId; // ID dell'oggetto
    #endregion

    [Header("References")]
    #region References
    public Tooltip tooltip; // Riferimento al tooltip
    private Animator animator; // Riferimento all'Animator del giocatore
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    private GamePlayMenuManager gamePlayMenuManager; // Riferimento allo script GamePlayMenuManager
    private OpenInventory openMenuScript; // Riferimento allo script OpenMenu
    #endregion

    void Start()
    {
        gamePlayMenuManager = FindObjectOfType<GamePlayMenuManager>();
        item = GetComponent<ItemController>().item;

        var player = FindObjectOfType<Player>().gameObject;
        openMenuScript = player.GetComponent<OpenInventory>();
        animator = player.GetComponent<Animator>();

        // Verifica se l'oggetto è stato raccolto
        itemId = GenerateItemId();
        if (GestoreScena.collectedItemIds.Contains(itemId)) Destroy(gameObject); // Distruggi l'oggetto se è già stato raccolto}
    }

    void Update()
    {
        if (gamePlayMenuManager.isMenuOpen) return; // Se il menu è aperto, non fare nulla

        if (isPlayerInRange && item.isPickUp) // Se è vicino a un oggetto raccoglibile
        {
            if (!animator.GetBool("pickingUp") && Input.GetKeyDown(KeyCode.Space))
            {
                if (item.inventorySectionType.Equals(Item.ItemType.ConsumableEquipable) && InventoryManager.instance.IsInventoryFull())
                { tooltip?.ShowTooltip("Non ho più spazio nello zaino.", 5f); return; }
                PickUp();
            }
            else if (animator.GetBool("pickingUp") && (animator.GetFloat("vInput") > 0 || animator.GetFloat("hInput") > 0)) CancelPickup();
        }
    }

    private string GenerateItemId()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string objectName = gameObject.name;
        Vector3 position = transform.position;
        return $"{sceneName}_{objectName}_{position.x}_{position.y}_{position.z}";
    }

    // private bool CheckClickMouseItem()
    // { ex: if (Input.GetMouseButtonDown(0) && CheckClickMouseItem()) PickUp();
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
                    (item as Weapon).prefab = Instantiate(gameObject);
                    (item as Weapon).prefab.SetActive(false);
                }
                InventoryManager.instance.Add(item);
                InventoryUIController.instance.ListItems(InventoryManager.instance.items);
            }
            FindObjectOfType<GestoreScena>().SetItemScene(item);
            isItemAdded = true;

            FindObjectOfType<Player>().GetComponent<ItemDetector>().RemoveItemDetection(gameObject);
            if (!GestoreScena.collectedItemIds.Contains(itemId)) GestoreScena.collectedItemIds.Add(itemId);
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
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}
