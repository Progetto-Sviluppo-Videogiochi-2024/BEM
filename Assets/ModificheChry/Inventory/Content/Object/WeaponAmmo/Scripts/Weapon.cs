using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Create New Weapon")]
public class Weapon : Item // Weapon estende Item
{
    [Header("Info Weapon")]
    public GameObject prefab; // Prefab dell'arma
    public WeaponType weaponType; // Tipo di arma (da mischia, da fuoco)
    public RangeType rangeType; // Tipo di raggio (corto, medio)
    public Ammo ammo; // Munizioni dell'arma
    public int bulletConsumed; // Munizioni consumate
    public int currentAmmo; // Munizioni attuali nel caricatore
    public int extraAmmo; // Munizioni extra (escluse quelle nel caricatore) (per il caricamento)
    public float distance; // Distanza massima di tiro dell'arma
    public bool semiAuto; // Se l'arma è semiautomatica (cioè spara un colpo per volta)
    public bool isThrowable; // Se l'arma è da lancio (coltelli, bottiglie)
    public bool isLoadingSlot; // Flag per il caricamento dello slot

    [Header("Positioning Weapon in Hands")]
    public Vector3 IdlePosition; // Posizione dell'arma equipaggiata
    public Quaternion IdleRotation; // Rotazione dell'arma equipaggiata
    public Vector3 AimPosition; // Posizione dell'arma equipaggiata
    public Quaternion AimRotation; // Rotazione dell'arma equipaggiata
    public Vector3 Scale; // Scala dell'arma equipaggiata

    public enum WeaponType
    {
        None, // Nessun tipo di arma
        Pistol, // Pistola
        Shotgun, // Fucile a pompa
        Rifle, // Fucile
        Sniper // Cecchino (fucile di precisione)
    }

    public enum RangeType
    {
        ShortRange, // Corto raggio (pistole, fucili a pompa)
        MediumRange, // Medio raggio (fucili d'assalto)
        LargeRange // Lungo raggio (fucili di precisione)
    }

    public void Initialize(WeaponData weaponData)
    {
        nameItem = weaponData.nameItem;
        description = weaponData.description;
        tagType = weaponData.tagType;
        value = weaponData.value;
        valueSanita = weaponData.valueSanita;
        effectType = weaponData.effectType;
        weight = weaponData.weight;
        ingredientsRecipe = weaponData.ingredientsRecipe;
        qtaIngredientsRecipe = weaponData.qtaIngredientsRecipe;
        craftItem = !string.IsNullOrEmpty(weaponData.craftItem) ? ItemManager.Instance.GetItemByName(weaponData.craftItem) : null;
        isUsable = weaponData.isUsable;
        isShooting = weaponData.isShooting;
        isPickUp = weaponData.isPickUp;
        canDestroy = weaponData.canDestroy;
        isInCraft = weaponData.isInCraft;
        isStackable = weaponData.isStackable;
        inventorySectionType = weaponData.inventorySectionType;
        own = weaponData.own;
        qta = weaponData.qta;
        var sprites = SpriteManager.Instance.GetSprites(nameItem);
        icon = sprites.icon;
        image = sprites.image;
        weaponType = weaponData.weaponType;
        rangeType = weaponData.rangeType;
        ammo = InventoryManager.instance.SearchItem(weaponData.ammoName) as Ammo ?? ItemManager.Instance.GetItemByName(weaponData.ammoName) as Ammo;
        distance = weaponData.distance;
        semiAuto = weaponData.semiAuto;
        isThrowable = weaponData.isThrowable;
        IdlePosition = weaponData.IdlePosition;
        IdleRotation = weaponData.IdleRotation;
        AimPosition = weaponData.AimPosition;
        AimRotation = weaponData.AimRotation;
        Scale = weaponData.Scale;
        isLoadingSlot = true;
        bulletConsumed = weaponData.bulletConsumed;
        currentAmmo = weaponData.currentAmmo;
        extraAmmo = weaponData.extraAmmo;
    }

    public void GetUpdateWAmmo(Item item) => prefab.GetComponent<WeaponAmmo>().UpdateAmmo(item as Ammo, true);
}
