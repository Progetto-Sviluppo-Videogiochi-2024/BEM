using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Item;

public class InventoryManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static InventoryManager instance; // Istanza statica dell'InventoryManager

    [Header("UI Components")]
    public GameObject inventoryPanel; // Pannello dell'inventario
    public Transform itemContent; // Contenitore degli oggetti nell'inventario

    [Header("Inventory Data")]
    public List<Item> items = new(); // Lista degli oggetti nell'inventario
    public InventoryItemController[] inventoryItems; // Array degli oggetti nell'inventario
    public List<Item> inventoryItemsToRemove; // Lista degli oggetti da rimuovere dall'inventario
    public Item itemEquipable; // Oggetto equipaggiabile
    public List<Item> weaponsEquipable = new(); // Lista delle armi equipaggiabili

    [Header("References Scripts")]
    [HideInInspector] public WeaponClassManager weaponClassManager; // Script che gestisce le armi equipaggiabili
    private ItemClassManager itemClassManager; // Script che gestisce gli oggetti consumabili

    void Awake()
    {
        var inventory = transform.parent.gameObject;

        if (instance != null && instance != this)
        {
            print("There is more than one instance of InventoryManager. Destroying duplicate.");
            Destroy(inventory); // Distrugge eventuali duplicati
            return;
        }

        instance = this;
        DontDestroyOnLoad(inventory);
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded; // Iscrizione all'evento

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded; // Disiscrizione all'evento

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu" || scene.name == "Transizione" || scene.name == "Scena3Video" || scene.name == "Scena1") return;

        var player = GameObject.FindGameObjectWithTag("Player");
        itemClassManager = player.GetComponent<ItemClassManager>();
        weaponClassManager = player.GetComponent<WeaponClassManager>();
        player.GetComponent<OpenInventory>().inventoryCanvas = transform.parent.GetChild(0).gameObject;
        inventoryItemsToRemove = new();
    }

    public Item CreateItemInstance(ItemData itemData)
    {
        // Crea una nuova istanza di Item o delle sue sottoclassi
        Item itemInstance = null;
        itemInstance = itemData.type switch
        {
            "Weapon" => ScriptableObject.CreateInstance<Weapon>(),// Crea una nuova istanza di Weapon
            "Ammo" => ScriptableObject.CreateInstance<Ammo>(),// Crea una nuova istanza di Ammo
            _ => ScriptableObject.CreateInstance<Item>(),// Crea una nuova istanza di Item
        };

        // Inizializza l'oggetto Item con i dati
        itemInstance.Initialize(itemData);
        return itemInstance;
    }

    public void ClearInventory()
    {
        // Pulisce la lista degli oggetti nell'inventario
        items.Clear();

        // Pulisce la UI degli oggetti nell'inventario
        foreach (Transform child in itemContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void Add(Item item)
    {
        // Controlla se l'oggetto esiste già nell'inventario
        Item existingItem = items.Find(i => i.nameItem == item.nameItem);

        if (existingItem != null) // Se già esiste
        {
            existingItem.qta++;
        }
        else // Se non esiste
        {
            item.qta = 1;
            items.Add(item);
            item.own = true;
        }
    }

    public void Remove(Item item, bool canDecreaseQta)
    {
        if (item == null) return;

        if (item.qta > 0 && canDecreaseQta)
        {
            item.qta--;
        }

        if (item.qta == 0)
        {
            item.own = false;
            items.Remove(item);
            UnequipItemPlayer(item);

            foreach (Transform child in itemContent)
            {
                if (child.GetComponent<InventoryItemController>().item == item)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
            SetInventoryItems();
        }
        InventoryUIController.instance.UpdateWeightSlider();
    }

    public void SetInventoryItems() => inventoryItems = itemContent.GetComponentsInChildren<InventoryItemController>();

    public void SortItems()
    {
        // Definiamo un dizionario per la priorità dei tag
        Dictionary<ItemTagType, int> priority = new()
        {
            { ItemTagType.Item, 1 },
            { ItemTagType.Weapon, 2 },
            { ItemTagType.Ammo, 3 },
            { ItemTagType.HiddenObject, 4 },
            { ItemTagType.Document, 5 },
            { ItemTagType.Recipe, 6 },
            { ItemTagType.Key, 7 }
        };

        // Ordinare gli oggetti in base alla priorità
        items = items.Where(item => priority.ContainsKey(item.tagType))
                    .OrderBy(item => priority[item.tagType])
                    .ToList();
    }

    public void EnableItemsRemove()
    {
        var inventoryActiveSection = InventoryUIController.instance.GetActiveSection();
        if (InventoryUIController.instance.enableRemove.isOn)
        {
            foreach (Transform item in itemContent)
            {
                if (inventoryActiveSection != item.GetComponent<InventoryItemController>().item.inventorySectionType.ToString())
                {
                    item.Find("ToggleRemoveItem").GetComponent<Toggle>().isOn = false;
                    continue;
                }
                item.Find("ToggleRemoveItem").GetComponent<Toggle>().isOn = true;
                if (!inventoryItemsToRemove.Contains(item.GetComponent<InventoryItemController>().item))
                    inventoryItemsToRemove.Add(item.GetComponent<InventoryItemController>().item);
            }
        }
        else
        {
            foreach (Transform item in itemContent)
            {
                item.Find("ToggleRemoveItem").GetComponent<Toggle>().isOn = false;
                //if (inventoryActiveSection != item.GetComponent<InventoryItemController>().item.inventorySectionType.ToString()) continue;
                if (inventoryItemsToRemove.Contains(item.GetComponent<InventoryItemController>().item))
                    inventoryItemsToRemove.Remove(item.GetComponent<InventoryItemController>().item);
            }
        }
    }

    public void RemoveEnableItems()
    {
        // Prendi la sezione attiva selezionata
        string inventoryActiveSection = InventoryUIController.instance.GetActiveSection();

        // Rimuovi gli oggetti presenti nella lista degli oggetti dell'inventario e nella lista degli oggetti da rimuovere
        foreach (Transform item in itemContent)
        {
            var itemRemove = item.GetComponent<InventoryItemController>().item;
            if (inventoryActiveSection != itemRemove.inventorySectionType.ToString()) continue; // Salta l'item se non appartiene alla sezione selezionata
            if (inventoryItemsToRemove.Contains(itemRemove))
            {
                items.Remove(itemRemove);
                inventoryItemsToRemove.Remove(itemRemove);
                Destroy(item.gameObject);
                UnequipItemPlayer(itemRemove);
                InventoryUIController.instance.UpdateWeightSlider();
            }
        }

        // Aggiorna l'array di InventoryItemController, filtrando quelli che hanno ancora un riferimento a un item valido
        inventoryItems = itemContent.GetComponentsInChildren<InventoryItemController>()
                                    .Where(invItem => invItem.item != null)  // Mantieni solo quelli con un item valido
                                    .ToArray();

        // Ricostruisci la lista degli item nel pannello
        InventoryUIController.instance.ListItems(items);
    }

    public void EquipItemPlayer(Item item)
    {
        if (item.tagType == ItemTagType.Weapon)
        {
            var weaponManager = (item as Weapon).prefab.GetComponent<WeaponManager>();
            weaponsEquipable.Add(item);
            if (!weaponClassManager.weaponsEquipable.Contains(weaponManager)) weaponClassManager.weaponsEquipable.Add(weaponManager);
            if (weaponsEquipable.Count == 1) weaponClassManager.ActiveAnimationWeapon(0);
            else if (weaponClassManager.currentWeapon == null) weaponClassManager.ActiveAnimationWeapon(weaponsEquipable.Count - 1);
            // Else if ho un'arma in mano => non fare nulla
        }
        else if (item.tagType == ItemTagType.Item)
        {
            var invItemContr = SearchItem(item);
            if (itemClassManager.itemEquipable != null && invItemContr != itemClassManager.itemEquipable) itemClassManager.itemEquipable.EquipItem(); // Disequipaggia l'item prima equipaggiato
            itemEquipable = item;
            itemClassManager.itemEquipable = invItemContr; // Equipaggia il nuovo item
        }
    }

    public void UnequipItemPlayer(Item item)
    {
        if (item.tagType == ItemTagType.Weapon && weaponsEquipable.Contains(item))
        {
            weaponsEquipable.Remove(item);
            weaponClassManager.RemoveEquipable(item);
        }
        else if (item.tagType == ItemTagType.Item && itemEquipable == item)
        {
            itemEquipable = null;
            itemClassManager.itemEquipable = null;
        }
    }

    private InventoryItemController SearchItem(Item item)
    {
        foreach (var inventoryItem in inventoryItems)
        {
            if (inventoryItem == null) continue;
            if (inventoryItem.item == item) return inventoryItem;
        }
        return null; // Non dovrebbe mai accadere credo
    }

    public int GetQtaItem(string ItemName)
    {
        foreach (var item in items)
        {
            if (item.nameItem == ItemName) return item.qta;
        }
        return 0;
    }

    public float ComputeInventoryWeight() =>
        items
            .Where(item => item.inventorySectionType != ItemType.Collectibles)
            .Sum(item => item.weight * item.qta);

    public bool IsInventoryFull() => ComputeInventoryWeight() >= InventoryUIController.instance.weightSlider.maxValue;
}
