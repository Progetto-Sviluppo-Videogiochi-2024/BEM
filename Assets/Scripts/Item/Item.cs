using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Create New Item")]
public class Item : ScriptableObject
{
    [Header("Info Object")]
    public int id; // ID dell'oggetto
    public int value; // Valore dell'oggetto
    public int qta = 0; // Quantità dell'oggetto

    public string nameItem; // Nome dell'oggetto
    public string description; // Descrizione dell'oggetto
    public string ingredientsRecipe; // Ingredienti per la creazione dell'oggetto, separati da ','
    public string qtaIngredientsRecipe; // Quantità degli ingredienti per la creazione dell'oggetto, separati da ','

    public Sprite icon = null; // Icona dell'oggetto
    public Sprite image = null; // Immagine dell'oggetto
    public GameObject prefab; // Prefab dell'oggetto
    public GameObject craftPrefab; // Prefab dell'oggetto da creare

    public float weight; // Peso dell'oggetto

    public bool own = false; // Possesso dell'oggetto (true se posseduto, false se non posseduto o non si può possedere)
    public bool isUsable; // Usabilità dell'oggetto (true se usabile, false se non usabile)
    public bool isStackable; // Stackabilità dell'oggetto (true se stackabile nell'inventario, false se non stackabile)
    public bool isShooting; // Sparabilità dell'oggetto (true se sparabile, false se non sparabile)
    public bool isPickUp; // Raccoglibilità dell'oggetto (true se raccoglibile, false se non raccoglibile)

    public ItemType inventorySectionType; // Tipo della sezione dell'inventario
    public ItemEffectType effectType; // Tipo di effetto dell'oggetto
    public ItemTagType tagType; // Tipo di tag dell'oggetto

    public enum ItemType // Enumerazione dei tipi di oggetti nell'inventario del giocatore (consumabili-equipaggiabili, collezionabili)
    {
        ConsumableEquipable,
        Collectibles
    }

    public enum ItemEffectType // Enumerazione dei tipi di effetti degli oggetti sul personaggio del giocatore
    {
        None,        // Nessun effetto
        Health,       // Effetto sulla vita
        Stamina,      // Effetto sulla stamina
        MentalHealth, // Effetto sulla sanità mentale
        // TODO: Aggiungerne altri
    }

    public enum ItemTagType // Tipo di tag dell'oggetto
    {
        Item,
        Weapon,
        Ammo,
        Key,
        Collectible,
        Document,
        Recipe,
        // altro ?
    }
}
