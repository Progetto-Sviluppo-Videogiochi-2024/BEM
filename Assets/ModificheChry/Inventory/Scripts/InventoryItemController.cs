using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Item;

public class InventoryItemController : MonoBehaviour, IPointerClickHandler
{
    [Header("Inventory Item Info Item")]
    public Item item; // L'oggetto dell'inventario
    private static InventoryItemController currentlySelectedItem; // L'oggetto attualmente selezionato

    [Header("Inventory Item Sub-menu Options")]
    private GameObject optionsMenu = null; // Il menu contestuale dell'oggetto cliccato (consumabile-equipaggiabile o collezionabile)
    private bool isOptionsOpen = false; // Stato attuale del menu contestuale

    [Header("Inspect Item Sub-menu Options")]
    public Transform inspectMenu; // Il menu Ispeziona dell'item/collezionabile
    public int indexIngredientCraft = 0; // Indice dell'ingrediente per la creazione dell'oggetto
    private List<string> ingredients = new(); // Lista degli ingredienti della ricetta
    private List<string> qtaIngredients = new(); // Lista delle quantità degli ingredienti della ricetta
    public Sprite imgDefault; // Immagine di default per l'ingrediente

    [Header("References")]
    [HideInInspector] public Tooltip tooltip; // Riferimento al tooltip
    private OpenInventory openMenuScript; // Riferimento allo script OpenMenu

    void Start()
    {
        openMenuScript = FindObjectOfType<Player>().GetComponent<OpenInventory>();

        var invUIController = InventoryUIController.instance;
        string activeSection = invUIController.GetActiveSection();

        if (activeSection == ItemType.ConsumableEquipable.ToString())
        {
            var nameMenu = "";
            switch (item.tagType)
            {
                case ItemTagType.Item:
                    if (item.isInCraft) nameMenu = "FeaturesMenuItemCraft";
                    else nameMenu = "FeaturesMenuItem";
                    break;

                case ItemTagType.Weapon:
                    nameMenu = "FeaturesMenuWeapon";
                    break;

                case ItemTagType.Ammo:
                    nameMenu = "FeaturesMenuAmmo";
                    break;

                case ItemTagType.Key:
                case ItemTagType.Document:
                case ItemTagType.HiddenObject:
                case ItemTagType.Recipe:
                    break; // Perché non hanno un menu contestuale

                default:
                    Debug.LogError("TagType non supportato: " + item.tagType); // TODO: non dovrebbe mai accadere
                    break;
            }
            if (!string.IsNullOrEmpty(nameMenu))
            {
                optionsMenu = transform.Find(nameMenu).gameObject;
                optionsMenu.SetActive(isOptionsOpen);
            }
        }
        else if (activeSection == ItemType.Collectibles.ToString())
        {
            optionsMenu = null;
            GetComponent<Button>().onClick.AddListener(InspectItem);
        }
        else // Se non è nessuna delle sezioni sopra, mostra un errore
        {
            Debug.LogError("Active section not found: " + activeSection);
        }

        inspectMenu = item.tagType == ItemTagType.Recipe ? invUIController.inspectMenuRecipe : invUIController.inspectMenuItem;

        if (item.tagType == ItemTagType.Recipe)
        {
            ingredients = GetIngredientsRecipe(item.ingredientsRecipe);
            qtaIngredients = GetIngredientsRecipe(item.qtaIngredientsRecipe);
        }
    }

    private void Update()
    {
        // Rinizializzo a 0 indexIngredientCraft se è una ricetta e ho chiuso il menu ispeziona
        if (item.tagType == ItemTagType.Recipe && !IsInspectItemUIActive()) indexIngredientCraft = 0;

        // Chiusura dei menu a seconda delle situazioni
        DetectClickOutsideMenuOrInspect();

        // Aggiornare a runtime il menu ispeziona delle ricette
        UpdateRecipeUI();
    }

    /* Gestione del click */
    private void DetectClickOutsideMenuOrInspect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // RectTransform di ispeziona e menu contestuale
            RectTransform optionsRect = optionsMenu != null ? optionsMenu.GetComponent<RectTransform>() : null;
            RectTransform inspectRect = inspectMenu != null ? inspectMenu.GetComponent<RectTransform>() : null;

            // Controllo il click per ciascuno dei menu
            Vector2 mousePosition = Input.mousePosition;
            bool clickedOnInspect = IsInspectItemUIActive() && RectTransformUtility.RectangleContainsScreenPoint(inspectRect, mousePosition);
            bool clickedOnOptions = optionsRect != null && RectTransformUtility.RectangleContainsScreenPoint(optionsRect, mousePosition);

            // Procedura per chiudere i vari menu
            if (clickedOnOptions) return; // Se clicco sul menu contestuale, non fare nulla
            if (clickedOnInspect) { OpenCloseOptions(false); return; } // Se clicco sull'ispeziona, chiudi il menu contestuale
            CloseAllMenus(); // Se dentro o fuori l'inventario, chiudi menu contestuale e ispeziona
        }
    }

    private void CloseAllMenus()
    {
        if (isOptionsOpen)
        {
            if (currentlySelectedItem != null) { currentlySelectedItem.OpenCloseOptions(false); currentlySelectedItem = null; }
            else OpenCloseOptions(false);
        }
        if (IsInspectItemUIActive()) OpenCloseInspectUI(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) // Se click col tasto sinistro
        {
            if (currentlySelectedItem != null) // Se c'è un item selezionato
            {
                if (currentlySelectedItem == this) // Se clicca sullo stesso item
                {
                    currentlySelectedItem.OpenCloseOptions(false);
                    currentlySelectedItem = null;
                    return;
                }
                else // Se è un altro
                {
                    currentlySelectedItem.OpenCloseOptions(false);
                    this.OpenCloseOptions(true);
                    currentlySelectedItem = this;
                }
            }
            else // Se non c'è un item selezionato
            {
                currentlySelectedItem = this;
                OpenCloseOptions(true);
            }
        }
    }

    /* Menu contestuale */
    public void OpenCloseOptions(bool open)
    {
        // Mostra o nascondi il menu contestuale dell'item cliccato
        if (optionsMenu != null)
        {
            if (isOptionsOpen) optionsMenu.transform.SetAsLastSibling(); // Porta il menu in primo piano
            isOptionsOpen = open;
            optionsMenu.SetActive(isOptionsOpen);
            ToggleInventoryItemsRaycast(!isOptionsOpen);
        }
    }

    /* Funzioni per le azioni dell'item */
    private void ToggleInventoryItemsRaycast(bool enable)
    {
        // Trova tutti gli item nell'inventario e abilita/disabilita il Raycast Target
        foreach (Transform item in InventoryManager.instance.inventoryPanel.transform)
        {
            // Assicurati di accedere al componente Image o Text dell'item
            if (item.TryGetComponent<Image>(out var itemImage))
            {
                itemImage.raycastTarget = enable;
            }

            // Se gli item contengono anche un TextMeshPro, puoi gestirlo qui
            if (item.TryGetComponent<TextMeshProUGUI>(out var itemText))
            {
                itemText.raycastTarget = enable;
            }
        }
    }

    public void EnableItemRemove()
    {
        var invMan = InventoryManager.instance;
        foreach (Transform item in invMan.itemContent)
        {
            // Ottieni l'item associato all'InventoryItemController di questo Transform
            var itemRemove = item.GetComponent<InventoryItemController>().item;

            // Confronta l'item attuale dello script con quello del foreach
            if (this.item == itemRemove)
            {
                if (!item.Find("ToggleRemoveItem").TryGetComponent<Toggle>(out var toggleRemove))
                {
                    Debug.LogError("ToggleRemoveItem not found!");
                    return;
                }
                if (toggleRemove.isOn && !invMan.inventoryItemsToRemove.Contains(itemRemove))
                    invMan.inventoryItemsToRemove.Add(itemRemove);
                else if (!toggleRemove.isOn && invMan.inventoryItemsToRemove.Contains(itemRemove))
                    invMan.inventoryItemsToRemove.Remove(itemRemove);
                break;
            }
        }
    }

    public void AddItem(Item item)
    {
        this.item = item;
    }

    private bool IsEquipable()
    {
        // Controlla se l'oggetto è equipaggiabile
        return item.own && item.qta > 0 && item.inventorySectionType == ItemType.ConsumableEquipable;
    }

    private bool IsAlsoConsumable()
    {
        // Controlla se l'oggetto è equipaggiabile e consumabile
        return IsEquipable() && item.isUsable && item.isStackable && item.tagType == ItemTagType.Item;
    }

    /* Ispeziona */
    public void InspectItem()
    {
        // Nascondi il menu ispeziona all'inizio
        OpenCloseInspectUI(false);

        // Mostra il menu appropriato dell'item cliccato
        ShowItemMenu();

        openMenuScript.itemInspectOpen = this;
        Debug.Log("inspectitem = " + openMenuScript.itemInspectOpen.item.nameItem);
    }

    private void ShowItemMenu()
    {
        OpenCloseInspectUI(true);
        inspectMenu.Find("NameItem").GetComponent<TextMeshProUGUI>().text = item.nameItem;
        inspectMenu.Find("ImgItem").GetComponent<Image>().sprite = item.image;
        inspectMenu.Find("DescriptionItem").GetComponent<TextMeshProUGUI>().text = item.description;

        if (item.tagType == ItemTagType.Recipe)
        {
            ShowCreateItemOption();
            return;
        }
        // Non mostrare i seguenti campi per le ricette

        // Gestisci quantità e peso
        var qta = inspectMenu.Find("QtaItem");
        var weight = inspectMenu.Find("WeightItem");

        if (item.tagType == ItemTagType.Item || item.tagType == ItemTagType.Weapon)
        {
            qta.gameObject.SetActive(true);
            qta.GetComponent<TextMeshProUGUI>().text = "Quantità: " + item.qta.ToString();

            weight.gameObject.SetActive(true);
            weight.GetComponent<TextMeshProUGUI>().text = "Peso: " + item.weight.ToString() + " kg";
        }
        else
        {
            qta.gameObject.SetActive(false);
            weight.gameObject.SetActive(false);
        }
    }

    private void ShowCreateItemOption()
    {
        // Mostra il primo ingrediente della ricetta
        ShowIngredientRecipeUI(indexIngredientCraft);
        if (ingredients.Count > 1)
        {
            inspectMenu.Find("SliderShowIngredientsRecipe/PreviousIngredient").GetComponent<Button>().onClick.AddListener(() => ShowIngredientRecipeUI(indexIngredientCraft - 1));
            inspectMenu.Find("SliderShowIngredientsRecipe/NextIngredient").GetComponent<Button>().onClick.AddListener(() => ShowIngredientRecipeUI(indexIngredientCraft + 1));
        }
        else // Se c'è un solo ingrediente, nascondi le frecce
        {
            inspectMenu.Find("SliderShowIngredientsRecipe/PreviousIngredient").gameObject.SetActive(false);
            inspectMenu.Find("SliderShowIngredientsRecipe/NextIngredient").gameObject.SetActive(false);
        }

        // Gestisci la creazione dell'oggetto
        var create = inspectMenu.Find("CreateItem");
        create.GetComponent<Button>().onClick.RemoveAllListeners(); // Rimuovi listener precedenti
        create.GetComponent<Button>().onClick.AddListener(CreateItem());
    }

    private void ShowIngredientRecipeUI(int indexIngredient)
    {
        if (ingredients.Count != qtaIngredients.Count)
        {
            Debug.LogError("List of ingredients and list of quantity ingredients are not the same length! Check the inspector of the item recipe");
            return;
        }

        // Gestisco l'indice dell'ingrediente per i 3 casi: per i click sulle frecce
        if (indexIngredient < 0) indexIngredientCraft = ingredients.Count - 1;
        else if (indexIngredient == ingredients.Count) indexIngredientCraft = 0;
        else indexIngredientCraft = indexIngredient;

        // Mostro l'ingrediente corrente
        foreach (var item in InventoryManager.instance.items)
        {
            if (item.nameItem == ingredients[indexIngredientCraft]) // Se l'item è un ingrediente e io ce l'ho
            {
                inspectMenu.Find("SliderShowIngredientsRecipe/NameIngredient").GetComponent<TextMeshProUGUI>().text = item.nameItem;
                inspectMenu.Find("SliderShowIngredientsRecipe/ImgIngredient").GetComponent<Image>().sprite = item.image;
                inspectMenu.Find("SliderShowIngredientsRecipe/QtaIngredient").GetComponent<TextMeshProUGUI>().text =
                    item.qta.ToString() + " / " + qtaIngredients[indexIngredientCraft].ToString();
                break;
            }
            else // Se non ho quell'ingrediente
            {
                inspectMenu.Find("SliderShowIngredientsRecipe/NameIngredient").GetComponent<TextMeshProUGUI>().text = ingredients[indexIngredientCraft];
                inspectMenu.Find("SliderShowIngredientsRecipe/ImgIngredient").GetComponent<Image>().sprite = imgDefault;
                inspectMenu.Find("SliderShowIngredientsRecipe/QtaIngredient").GetComponent<TextMeshProUGUI>().text =
                    "0 / " + qtaIngredients[indexIngredientCraft].ToString();
                break;
            }
        }
    }

    private List<string> GetIngredientsRecipe(string input)
    {
        if (input.Contains(","))
        {
            return input.Split(',')
                        .Select(element => element.Trim())
                        .Where(element => !string.IsNullOrEmpty(element))
                        .ToList();
        }
        else // Se non ci sono virgole, c'è un solo ingrediente
        {
            return new List<string> { input.Trim() };
        }
    }

    public void OpenCloseInspectUI(bool openMenu)
    {
        inspectMenu.gameObject.SetActive(openMenu);
    }

    private bool IsInspectItemUIActive()
    {
        return inspectMenu.gameObject.activeSelf;
    }

    /* Usa */
    public void UseItem(GameObject buttonClicked)
    {
        print(tooltip == null ? "Tooltip not found" : tooltip.name);
        var button = buttonClicked.GetComponent<Button>();
        if (IsAlsoConsumable())
        {
            var player = FindObjectOfType<Player>();
            if (player.health >= player.maxHealth && player.sanitaMentale >= player.maxHealth) { button.interactable = false; tooltip.ShowTooltip("Non ne ho bisogno!", 5f); return; }
            else button.interactable = true;
            if (item.effectType == ItemEffectType.Health) { player.UpdateHealth(item.value); RemoveItem(); }
        }
    }

    /* Scarta */
    public void RemoveItem()
    {
        if (IsEquipable() || IsAlsoConsumable())
        {
            InventoryManager.instance.Remove(item, true);
        }
    }

    /* Crea */
    private UnityAction CreateItem()
    {
        return () =>
        {
            // Prendere gli items necessari per il crafting (parametri della classe InventoryItemController)

            // Controllare se l'utente ha gli items necessari
            //var (canCraft, missingItem) = CanCraftItem();
            // if (!canCraft)
            // {
            //     ApplyEffectOnItemCreation(missingItem);
            //     // TODO: aggiungere SFX
            //     LeanTween.moveLocalX(inspectMenu.gameObject, inspectMenu.transform.localPosition.x + 10f, 0.1f).setLoopPingPong(2);
            //     return;
            // }

            // Crearlo e aggiungerlo all'inventario
            var invManager = InventoryManager.instance;
            invManager.Add(item.craftItem);

            // Togliere quelle qta dall'inventario (se qta = 0 rimuoverlo dall'inventario)
            foreach (var ingredient in ingredients)
            {
                var itemInInventory = invManager.items.Find(item => item.nameItem == ingredient);
                if (itemInInventory == null) return; // Non dovrebbe mai accadere
                itemInInventory.qta -= int.Parse(qtaIngredients[ingredients.IndexOf(ingredient)]);
                if (itemInInventory.qta == 0)
                {
                    invManager.Remove(itemInInventory, false);
                    if (invManager.inventoryItemsToRemove.Contains(itemInInventory)) invManager.inventoryItemsToRemove.Remove(itemInInventory);
                    if (invManager.itemEquipable == itemInInventory) invManager.itemEquipable = null;
                    else if (invManager.weaponsEquipable.Contains(itemInInventory)) invManager.weaponsEquipable.Remove(itemInInventory);
                }
            }

            // Aggiornare la UI
            InventoryUIController.instance.ListItems(invManager.items);

            if (IsInspectItemUIActive())
            {
                openMenuScript.itemInspectOpen = this;
            }
        };
    }

    private /*(bool, string)*/ bool CanCraftItem()
    {
        if (InventoryManager.instance.IsInventoryFull()) return false; // L'inventario è pieno

        for (int i = 0; i < ingredients.Count; i++)
        {
            var itemInInventory = InventoryManager.instance.items.Find(item => item.nameItem == ingredients[i]);

            // Se l'item non è trovato nell'inventario o non ha quantità sufficienti
            if (itemInInventory == null) return false; //(false, ingredients[i]);
            else if (itemInInventory.qta < int.Parse(qtaIngredients[i])) return false; //(false, itemInInventory.nameItem);
        }
        return true; //(true, null); // Tutti gli ingredienti sono presenti in quantità sufficienti
    }

    // private void ApplyEffectOnItemCreation(string missingItem)
    // {
    //     if (missingItem == ingredients[indexIngredientCraft])
    //     {
    //         inspectMenu.Find("SliderShowIngredientsRecipe/ImgIngredient").GetComponent<Image>().color = new(1, 1, 1, 0.5f);  // Trasparenza del 50%
    //         inspectMenu.Find("SliderShowIngredientsRecipe/QtaIngredient").GetComponent<TextMeshProUGUI>().color = Color.red;
    //     }
    // }

    private void UpdateRecipeUI()
    {
        if (IsInspectItemUIActive() && item.tagType == ItemTagType.Recipe)
        {
            ShowIngredientRecipeUI(indexIngredientCraft);
            //var (canCraft, _) = CanCraftItem();
            // if (canCraft)
            // {
            //     inspectMenu.Find("SliderShowIngredientsRecipe/ImgIngredient").GetComponent<Image>().color = new(1, 1, 1, 1); // Opacità al 100%
            //     inspectMenu.Find("SliderShowIngredientsRecipe/QtaIngredient").GetComponent<TextMeshProUGUI>().color = Color.white;
            // }

            var createButton = inspectMenu.Find("CreateItem").GetComponent<Button>();
            createButton.onClick.RemoveAllListeners(); // Rimuovi listener precedenti
            if (CanCraftItem())
            {
                createButton.onClick.AddListener(CreateItem());
                createButton.interactable = true; // Attiva il bottone se è possibile craftare l'oggetto
            }
            else createButton.interactable = false; // Disabilita il bottone se non è possibile craftare l'oggetto
        }
    }

    /* Equipaggia */
    public void EquipItem()
    {
        if (IsEquipable())
        {
            GameObject labelEquipable = transform.Find("LabelEquipable").gameObject;
            TextMeshProUGUI buttonTextEquip = transform.Find($"{optionsMenu.name}/EquipButton").GetComponent<Button>().GetComponentInChildren<TextMeshProUGUI>();
            if (transform.Find("LabelEquipable").gameObject.activeSelf) // Se è equipaggiato, per disequipaggiarlo
            {
                buttonTextEquip.text = "Equipaggia";
                InventoryManager.instance.UnequipItemPlayer(item);
            }
            else // Se non è equipaggiato, per equipaggiarlo
            {
                buttonTextEquip.text = "Disequipaggia";
                InventoryManager.instance.EquipItemPlayer(item);
            }
            labelEquipable.SetActive(!labelEquipable.activeSelf);
        }
    }
}
