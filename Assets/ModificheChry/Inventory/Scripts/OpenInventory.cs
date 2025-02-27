using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenInventory : MonoBehaviour
{
    [Header("Inventory")]
    #region Inventory
    public GameObject inventoryCanvas; // Il Canvas dell'inventario da cui prendo i suoi figli
    [HideInInspector] public bool isInventoryOpen = false; // Stato attuale dell'inventario
    #endregion

    [Header("References")]
    #region References
    [HideInInspector] public InventoryItemController itemInspectOpen; // L'oggetto con ispeziona aperto
    [HideInInspector] private Transform zoomMenu; // Il menu di zoom per i documenti
    #endregion

    void Start()
    {
        if (inventoryCanvas != null)
        {
            // Nascondi il Canvas dell'inventario all'avvio
            inventoryCanvas.SetActive(false);
            zoomMenu = inventoryCanvas.transform.Find("Zoom");

            // Per chiudere l'inventario e aggiungerne l'evento di chiusura al click sul bottone
            inventoryCanvas.transform.Find("Inventory/Close").GetComponent<Button>().onClick
                .AddListener(() => ToggleInventory(false));
        }
    }

    void Update()
    {
        if (isInventoryOpen) { ToggleCinematic(); }

        ActivateInventory(); // Se I, apri/chiudi l'inventario
    }

    private void ActivateInventory()
    {
        // if (PlayerPrefs.GetInt("hasBackpack") != 1) return; // TODO: IDK se farlo o meno

        if (Input.GetKeyDown(KeyCode.I))
        {
            var inventoryUIManager = InventoryUIController.instance;
            var inventoryManager = InventoryManager.instance;

            ToggleInventory(!isInventoryOpen);

            if (!isInventoryOpen && inventoryUIManager.enableRemove.isOn)
            {
                inventoryUIManager.enableRemove.isOn = false;
                inventoryManager.inventoryItemsToRemove.Clear();
            }
            else if (isInventoryOpen) // Se l'inventario è aperto
            {
                // Se non c'è una sezione attiva, selezionane una di default
                if (string.IsNullOrEmpty(inventoryUIManager.GetActiveSection()))
                {
                    // Seleziona la prima sezione disponibile
                    inventoryUIManager.SelectSectionInventory(inventoryUIManager.listPanelInventory[0]);
                }
                else // Se c'è una sezione attiva
                {
                    // Aggiorna la visualizzazione della sezione attiva
                    inventoryUIManager.ListItems(inventoryManager.items);
                }
            }
        }
    }

    public void ToggleInventory(bool isOpen)
    {
        // Cambia lo stato dell'inventario e visualizza o nascondi il Canvas
        isInventoryOpen = isOpen;
        inventoryCanvas.SetActive(isInventoryOpen);
        GestoreScena.ChangeCursorActiveStatus(isInventoryOpen, "Inventory");
        if (!isInventoryOpen) GetComponent<AimStateManager>().enabled = true;
        if (!isOpen && zoomMenu.gameObject.activeSelf) zoomMenu.gameObject.SetActive(false);

        // Chiudi il menu ispeziona se l'inventario viene chiuso
        if (isOpen && itemInspectOpen != null)
        {
            itemInspectOpen.OpenCloseInspectUI(false);
            itemInspectOpen = null;
        }
    }

    private void ToggleCinematic()
    {
        // Se il mouse è sopra un UI, disabilita la visuale
        bool isCursorOverUI = EventSystem.current.IsPointerOverGameObject();
        GetComponent<AimStateManager>().enabled = !isCursorOverUI;
    }
}
