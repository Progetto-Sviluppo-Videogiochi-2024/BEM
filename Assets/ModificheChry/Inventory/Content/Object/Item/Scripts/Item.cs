using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create New Item")]
public class Item : ScriptableObject
{
    [Header("General Info")]
    public string nameItem; // Nome dell'oggetto
    public string description; // Descrizione dell'oggetto
    public ItemTagType tagType; // Tipo di tag dell'oggetto

    [Header("Consumable-Equipable Info")]
    [Tooltip("Oggetti: Valore usato da effectType")]
    public int value; // Valore dell'oggetto
    public int valueSanita; // Valore della sanità mentale  

    [Tooltip("Tipo di effetto dell'oggetto, solo per gli oggetti consumabili")]
    public ItemEffectType effectType; // Tipo di effetto dell'oggetto
    public float weight; // Peso dell'oggetto

    [Header("Recipe Info")]
    [Tooltip("Nome degli ingredienti necessari alla creazione dell'oggetto (separati da ',')")]
    public string ingredientsRecipe; // Ingredienti per la creazione dell'oggetto, separati da ','

    [Tooltip("Quantità degli ingredienti necessari alla creazione dell'oggetto (separati da ',')")]
    public string qtaIngredientsRecipe; // Quantità degli ingredienti per la creazione dell'oggetto, separati da ','

    [Tooltip("Item.cs dell'oggetto da creare")]
    public Item craftItem; // Item.cs dell'oggetto da creare

    [Header("Usability Info")]
    public bool isUsable; // Usabilità dell'oggetto (true se usabile, false se non usabile)
    public bool isShooting; // Sparabilità dell'oggetto (true se sparabile, false se non sparabile)
    public bool isPickUp; // Raccoglibilità dell'oggetto (true se raccoglibile, false se non raccoglibile)
    public bool canDestroy; // Distruttibilità dell'oggetto (true se distruttibile, false se non distruttibile)

    [Tooltip("Presenza dell'oggetto nella lista degli oggetti craftabili")]
    public bool isInCraft; // Presenza dell'oggetto nella lista degli oggetti craftabili (true se presente, false se non presente)

    [Header("Inventory Info")]
    [Tooltip("Stackabilità = oggetto accumulabile in più quantità nello stesso slot dell'inventario")]
    public bool isStackable; // Stackabilità dell'oggetto (true se stackabile nell'inventario, false se non stackabile)
    public ItemType inventorySectionType; // Tipo della sezione dell'inventario
    public bool own = false; // Possesso dell'oggetto (true se posseduto, false se non posseduto o non si può possedere)
    public int qta = 0; // Quantità dell'oggetto
    public Sprite icon = null; // Icona dell'oggetto
    public Sprite image = null; // Immagine dell'oggetto

    public enum ItemType // Enumerazione dei tipi di oggetti nell'inventario del giocatore (consumabili-equipaggiabili, collezionabili)
    {
        ConsumableEquipable,
        Collectibles
    }

    public enum ItemEffectType // Enumerazione dei tipi di effetti degli oggetti sul personaggio del giocatore
    {
        None,        // Nessun effetto
        Health       // Effetto sulla vita
    }

    public enum ItemTagType // Tipo di tag dell'oggetto
    {
        Item, // Consumabili come cibo, medicine, ecc.
        Weapon, // Armi
        Ammo, // Munizioni
        Key, // Chiavi
        HiddenObject, // Oggetti nascosti
        Document, // Documenti
        Recipe, // Ricette
        Scene // Oggetti di scena ma non presenti nell'inventario
    }

    public virtual void Initialize(ItemData itemData)
    {
        nameItem = itemData.nameItem;
        description = itemData.description;
        tagType = itemData.tagType;
        value = itemData.value;
        valueSanita = itemData.valueSanita;
        effectType = itemData.effectType;
        weight = itemData.weight;
        ingredientsRecipe = itemData.ingredientsRecipe;
        qtaIngredientsRecipe = itemData.qtaIngredientsRecipe;
        craftItem = !string.IsNullOrEmpty(itemData.craftItem) ? ItemManager.Instance.GetItemByName(itemData.craftItem) : null;
        isUsable = itemData.isUsable;
        isShooting = itemData.isShooting;
        isPickUp = itemData.isPickUp;
        canDestroy = itemData.canDestroy;
        isInCraft = itemData.isInCraft;
        isStackable = itemData.isStackable;
        inventorySectionType = itemData.inventorySectionType;
        own = itemData.own;
        qta = itemData.qta;
        var sprites = SpriteManager.Instance.GetSprites(nameItem);
        icon = sprites.icon;
        image = sprites.image;
    }

    public override string ToString() => nameItem;
}
