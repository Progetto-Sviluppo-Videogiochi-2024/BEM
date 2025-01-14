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

    [Header("Camera Pickup Settings")]
    #region Camera Pickup Settings
    private Transform camFollowPosition;
    #endregion

    [Header("References")]
    #region References
    private Transform player;
    public Tooltip tooltip; // Riferimento al tooltip
    private Animator animator; // Riferimento all'Animator del giocatore
    #endregion

    [Header("References Scripts")]
    #region References Scripts
    private GamePlayMenuManager gamePlayMenuManager; // Riferimento allo script GamePlayMenuManager
    private OpenInventory openMenuScript; // Riferimento allo script OpenMenu
    #endregion

    void Awake()
    {
        player = FindObjectOfType<Player>().transform;
        camFollowPosition = player.GetChild(0);
        openMenuScript = player.GetComponent<OpenInventory>();
        animator = player.GetComponent<Animator>();
    }

    void Start()
    {
        gamePlayMenuManager = FindObjectOfType<GamePlayMenuManager>();
        item = GetComponent<ItemController>().item;

        // Verifica se l'oggetto è stato raccolto
        itemId = GestoreScena.GenerateId(gameObject, transform);
        if (GestoreScena.collectedItemIds.Contains(itemId))
        {
            if (item.tagType != Item.ItemTagType.Weapon) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (gamePlayMenuManager.isMenuOpen) return; // Se il menu è aperto, non fare nulla

        if (isPlayerInRange && item.isPickUp) // Se è vicino a un oggetto raccoglibile
        {
            if (!animator.GetBool("pickingUp") && Input.GetKeyDown(KeyCode.Space))
            {
                if (item.inventorySectionType.Equals(Item.ItemType.ConsumableEquipable) && InventoryManager.instance.IsInventoryFull())
                {
                    tooltip?.ShowTooltip("Non ho più spazio nello zaino.", 5f);
                    return;
                }
                PickUp();
            }
        }
    }

    private void PickUp()
    {
        animator.SetBool("pickingUp", true);
        if (openMenuScript.isInventoryOpen || openMenuScript.itemInspectOpen != null) openMenuScript.ToggleInventory(false);
        MoveCamerPickup(camFollowPosition);
        StartCoroutine(WaitForEquipAnimation());
    }

    private IEnumerator MoveCameraCoroutine(Transform cameraPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = cameraPosition.localPosition;
        float distanceToPlayer = Vector3.Distance(cameraPosition.position, player.position) * 1.5f;
        Vector3 targetPosition = new(startPosition.x, startPosition.y, -distanceToPlayer);

        while (elapsedTime < duration)
        {
            cameraPosition.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraPosition.localPosition = targetPosition; // Assicurati che la posizione finale sia precisa
        gameObject.SetActive(false);
    }

    private void MoveCamerPickup(Transform cameraPosition) => StartCoroutine(MoveCameraCoroutine(cameraPosition, 2f));

    // private void CancelPickup() => animator.SetBool("pickingUp", false);

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
                    SetWAmmo(item as Weapon);
                }
                else if (item.tagType == Item.ItemTagType.Ammo)
                {
                    var weapon = InventoryManager.instance.SearchWeapon(item as Ammo);
                    weapon?.GetUpdateWAmmo(item);
                }
                InventoryManager.instance.Add(item);
                InventoryUIController.instance.ListItems(InventoryManager.instance.items);
            }
            FindObjectOfType<GestoreScena>().SetItemScene(item);
            isItemAdded = true;

            player.GetComponent<ItemDetector>().RemoveItemDetection(gameObject);
            if (!GestoreScena.collectedItemIds.Contains(itemId)) GestoreScena.collectedItemIds.Add(itemId);
            if (item.canDestroy) Destroy(gameObject);
        }

        animator.SetBool("pickingUp", false);
        MoveCamerPickup(camFollowPosition);
    }

    private bool IsAnimationFinished(string layer, string animation, float normalizedTime)
    {
        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layer));
        return animStateInfo.IsName(animation) && animStateInfo.normalizedTime >= normalizedTime;
    }

    public void SetWAmmo(Weapon weapon)
    {
        var weaponAmmo = weapon.prefab.GetComponent<WeaponAmmo>();
        if (!InventoryManager.instance.SearchAmmo(weapon)) return;
        weaponAmmo.UpdateAmmo(weapon.ammo, false);
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
