using System.Collections;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Object by Pickup")]
    public Item item; // Oggetto da raccogliere

    [Header("Variables")]
    private bool isPlayerInRange; // Per sapere se il giocatore è vicino all'oggetto
    private bool isItemAdded = false; // Flag per assicurarsi che l'oggetto venga aggiunto solo una volta

    [Header("Renderer")]
    public Material outlineMaterial;  // Materiale con l'effetto outline
    private Material originalMaterial; // Materiale originale dell'oggetto
    private Renderer objectRenderer; // Riferimento al renderer dell'oggetto

    [Header("References")]
    private Animator animator; // Riferimento all'Animator del giocatore
    private OpenMenu openMenuScript;


    private void Start()
    {
        openMenuScript = FindObjectOfType<Player>().gameObject.GetComponent<OpenMenu>();
        animator = FindObjectOfType<Player>().gameObject.GetComponent<Animator>();

        InitMaterial();
    }

    private void Update()
    {
        if (isPlayerInRange && item.isPickUp) // Se è vicino a un oggetto raccoglibile
        {
            if (Input.GetMouseButtonDown(0) && CheckClickMouseItem()) PickUp();
            else if (!animator.GetBool("isPickingUp") && Input.GetKeyDown(KeyCode.Space)) PickUp();
            else if (animator.GetBool("isPickingUp") && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))) CancelPickup();
        }
    }

    private void InitMaterial()
    {
        // Ottieni il renderer dell'oggetto
        objectRenderer = GetComponent<Renderer>();

        // Memorizza il materiale originale dell'oggetto
        if (objectRenderer != null) originalMaterial = objectRenderer.material;
    }

    private bool CheckClickMouseItem()
    {
        // Raycast per determinare se il clic è effettivamente sull'oggetto
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform && !animator.GetBool("isPickingUp")) return true;
        }
        return false;
    }

    private void PickUp()
    {
        // Animo il giocatore per raccogliere l'oggetto
        animator.SetBool("isPickingUp", true);
        if (openMenuScript.isInventoryOpen || openMenuScript.itemInspectOpen != null)
        { openMenuScript.OpenCloseInventory(false); Debug.Log("Close Inventory"); }
        StartCoroutine(WaitForEquipAnimation());
    }

    private void CancelPickup()
    {
        animator.SetBool("isPickingUp", false);
    }

    private IEnumerator WaitForEquipAnimation()
    {
        // Attendi che l'animazione di equipaggiamento sia finita
        yield return new WaitUntil(() => IsAnimationFinished("Action", "Taking Item", 0.3f));

        // Verifica se l'oggetto è già stato aggiunto
        if (!isItemAdded && animator.GetBool("isPickingUp"))
        {
            if (item.tagType == Item.ItemTagType.Weapon)
            {
                objectRenderer.material = originalMaterial;
                item.prefab = Instantiate(gameObject);
                item.prefab.SetActive(false);
            }
            InventoryManager.instance.Add(item);
            InventoryUIController.instance.ListItems(InventoryManager.instance.items);
            isItemAdded = true;

            Destroy(gameObject);
        }

        animator.SetBool("isPickingUp", false);
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
            if (!isItemAdded) objectRenderer.material = outlineMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Controlla se il giocatore esce dal raggio di raccolta
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            objectRenderer.material = originalMaterial;
        }
    }
}
