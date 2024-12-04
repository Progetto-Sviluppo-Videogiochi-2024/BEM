using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Item;
using System.Linq;
using TMPro;

public class InventoryUIController : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static InventoryUIController instance; // Istanza statica dell'InventoryUIController list

    [Header("UI Components")]
    [HideInInspector] public Tooltip tooltip; // Tooltip per gli oggetti nell'inventario (ad es: inventario pieno)
    public GameObject inventoryPanel; // Pannello dell'inventario
    public Transform itemContent; // Contenitore degli oggetti nell'inventario
    public GameObject inventoryItemPrefab; // Prefab dell'oggetto nell'inventario
    public Toggle enableRemove; // Abilita la rimozione degli oggetti dall'inventario
    public List<GameObject> listPanelInventory; // Lista dei button figli di inventoryPanel che iniziano per "List"
    public Slider weightSlider; // Slider del peso dell'inventario
    public RectTransform inspectMenuItem; // Il menu Ispeziona dell'item/collezionabile con alcuni vincoli:
                                          // - item - armi: ma abilitare qta e peso
                                          // - chiavi - documenti - oggetti nascosti - ammo: ma disabilitare qta e peso
    public RectTransform inspectMenuRecipe; // Il menu Ispeziona della ricetta

    [Header("Inventory State")]
    private string currentActiveSection = ""; // Sezione attiva dell'inventario
    private readonly List<Color> outlineColors = new() // Lista dei colori per il contorno dei button (attivo e inattivi) dell'inventario
    {
        new(255f, 0f, 0f, 128f), // Rosso (attivo)
        new(0f, 0f, 0f, 128f)  // Nero (inattivo)
    };

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There is more than one instance of InventoryUIController in the scene.");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        listPanelInventory = GetButtonInventoryList();
        weightSlider.maxValue = 6.0f; // Peso massimo degli oggetti equipaggiabili
    }

    private List<GameObject> GetButtonInventoryList() =>
        inventoryPanel.transform
            .Cast<Transform>()
            .Where(child => child.gameObject.name.StartsWith("List"))
            .Select(child => child.gameObject)
            .ToList();

    public void ListItems(List<Item> items)
    {
        // Pulisci la vista dell'inventario (disattiva gli oggetti)
        items = items.Where(item => item != null).ToList();
        ClearInventoryView();

        // Aggiorna l'interfaccia utente dell'inventario
        InventoryManager.instance.SortItems();
        UpdateInventoryView(items, GetActiveSection());
    }

    private void ClearInventoryView()
    {
        // Distruggi tutti gli oggetti nell'inventario
        foreach (Transform item in itemContent) Destroy(item.gameObject);
    }

    private void UpdateInventoryView(List<Item> items, string inventoryActiveSection)
    {
        // Prendo la lista degli item attivi nella sezione selezionata
        List<Item> itemsActiveSection = GetItemsActiveSection(items, inventoryActiveSection)
                                            .Where(item => item != null)
                                            .ToList();

        bool isActiveList = inventoryActiveSection == ItemType.Collectibles.ToString();
        enableRemove.gameObject.SetActive(!isActiveList);

        // Ciclo su ogni item attivo nella sezione selezionata e li aggiungo all'inventario nella UI
        var invMan = InventoryManager.instance;
        foreach (var item in itemsActiveSection)
        {
            if (item == null) continue; // Salta l'item se è nullo

            // Istanzia un nuovo item nell'inventario con i dati dell'item attuale
            GameObject newItem = Instantiate(inventoryItemPrefab, itemContent);
            newItem.name = item.nameItem;
            newItem.transform.Find("NameItem").GetComponent<TextMeshProUGUI>().text = item.nameItem;
            newItem.transform.Find("IconItem").GetComponent<Image>().sprite = item.icon;
            var invItemCon = newItem.GetComponent<InventoryItemController>();
            invItemCon.item = item;
            invItemCon.tooltip = tooltip;
            invItemCon.inspectMenu = item.tagType == ItemTagType.Recipe ? inspectMenuRecipe : inspectMenuItem;

            // Per gestire il toggle dell'item in base alla sezione attiva
            if (isActiveList) // Se "Collectibles", impedisco la rimozione degli oggetti "Collectibles"
            {
                newItem.transform.Find("ToggleRemoveItem").gameObject.SetActive(!isActiveList);
            }
            else // Se "ConsumableEquipable", abilito la rimozione degli oggetti "ConsumableEquipable"
            {
                newItem.transform.Find("ToggleRemoveItem").GetComponent<Toggle>().isOn = enableRemove.isOn;
            }

            // Se l'item è equipaggiato
            if (invMan.itemEquipable == item || invMan.weaponsEquipable.Contains(item))
            {
                var nameMenu = invMan.itemEquipable == item ? "FeaturesMenuItem" : "FeaturesMenuWeapon";
                newItem.transform.Find("LabelEquipable").gameObject.SetActive(true);
                newItem.transform.Find($"{nameMenu}/EquipButton").GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>().text = "Disequipaggia";
            }
        }
        UpdateWeightSlider();

        // Imposta correttamente gli oggetti nell'inventario
        invMan.SetInventoryItems();
    }

    private List<Item> GetItemsActiveSection(List<Item> items, string inventoryActiveSection) =>
        items.Where(item => item.inventorySectionType.ToString() == inventoryActiveSection).ToList();

    public void SelectSectionInventory(GameObject clickedButton)
    {
        // Ottieni la sezione associata al button cliccato
        var selectedSection = RemoveListPrefix(clickedButton.name);

        // Controlla se la sezione selezionata è già attiva
        if (selectedSection == currentActiveSection) return; // Se è già attiva, non fare nulla (evita computazioni inutili)

        // Imposta il contorno attorno al button selezionato ed escludi il contorno attorno agli altri button
        ChangeButtonInventoryOutline(clickedButton);

        // Mostra gli oggetti dell'inventario relativi alla sezione selezionata
        ListItems(InventoryManager.instance.items);

        // Aggiorna la sezione attiva
        currentActiveSection = selectedSection;
    }

    public string GetActiveSection()
    {
        for (int i = 0; i < listPanelInventory.Count; i++)
        {
            if (listPanelInventory[i].GetComponent<Outline>().enabled) return RemoveListPrefix(listPanelInventory[i].name);
        }

        // Restituisci un valore di default se nessun bottone ha l'outline abilitato
        return string.Empty; // Non dovrebbe mai accadere
    }

    public string RemoveListPrefix(string input)
    {
        // Se inizia con "List"
        if (input.StartsWith("List")) return input["List".Length..]; // Rimuove il prefisso "List"

        // Else, restituisce la stringa originale
        return input; // Non dovrebbe mai accadere
    }

    private void ChangeButtonInventoryOutline(GameObject clickedButton)
    {
        // Imposta il contorno attorno al button selezionato
        string gameObjectClickedButton = clickedButton.name;
        Outline outline = clickedButton.GetComponent<Outline>();
        outline.enabled = true;
        outline.effectColor = outlineColors[0];

        // Escludi il contorno attorno agli altri button
        ExcludeOutline(gameObjectClickedButton);
    }

    private void ExcludeOutline(string gameObjectClickedButton)
    {
        // Escludi il contorno attorno agli altri button diversi da quello selezionato
        for (int i = 0; i < listPanelInventory.Count; i++)
        {
            if (listPanelInventory[i].name != gameObjectClickedButton)
            {
                Outline outline = listPanelInventory[i].GetComponent<Outline>();
                outline.enabled = false;
                outline.effectColor = outlineColors[1];
            }
        }
    }

    public void UpdateWeightSlider()
    {
        var weight = InventoryManager.instance.ComputeInventoryWeight();

        // Aggiorna lo slider del peso
        float weightPercentage = weight / weightSlider.maxValue;
        weightSlider.value = weight;

        // Cambia colore della barra
        if (weightPercentage <= 0.5f) weightSlider.fillRect.GetComponent<Image>().color = Color.green;
        else if (weightPercentage <= 0.8f) weightSlider.fillRect.GetComponent<Image>().color = new(1f, 0.64f, 0f); // Arancione
        else weightSlider.fillRect.GetComponent<Image>().color = Color.red;
    }
}
