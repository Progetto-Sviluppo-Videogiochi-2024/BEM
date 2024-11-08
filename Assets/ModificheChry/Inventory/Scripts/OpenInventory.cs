using UnityEngine;
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
    #endregion

    void Start()
    {
        if (inventoryCanvas != null)
        {
            // Nascondi il Canvas dell'inventario all'avvio
            inventoryCanvas.SetActive(false);

            // Per chiudere l'inventario e aggiungerne l'evento di chiusura al click sul bottone
            inventoryCanvas.transform.Find("Inventory/Close").GetComponent<Button>()
                .onClick.AddListener(() => OpenCloseInventory(false));
        }
    }

    void Update()
    {
        // Se il giocatore preme il tasto "I", apri o chiudi l'inventario
        ActivateInventory();
    }

    private void ActivateInventory()
    {
        // if (PlayerPrefs.GetInt("hasBackpack") != 1) return; // TODO: scommentare a fine gioco
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            var inventoryUIManager = InventoryUIController.instance;
            var inventoryManager = InventoryManager.instance;
            OpenCloseInventory(!isInventoryOpen);
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

    public void OpenCloseInventory(bool open)
    {
        // Cambia lo stato dell'inventario e visualizza o nascondi il Canvas
        isInventoryOpen = open;
        inventoryCanvas.SetActive(isInventoryOpen);

        // Chiudi il menu ispeziona se l'inventario viene chiuso
        if (!isInventoryOpen && itemInspectOpen != null)
        {
            // Debug.Log(itemInspectOpen.item.nameItem);
            itemInspectOpen.OpenCloseInspectUI(false);
            itemInspectOpen = null;
        }

        // TODO: anche eliminabile se non disabilitiamo il cursore del mouse
        // Cursor.visible = isInventoryOpen;
        // Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
