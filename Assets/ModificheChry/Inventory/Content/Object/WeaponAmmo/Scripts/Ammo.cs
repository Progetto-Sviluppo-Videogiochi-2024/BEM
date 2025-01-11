using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Create New Ammo")]
public class Ammo : Item
{
    [Header("Info Ammo")]
    // Prefab lo passo già a WeaponManager
    public AmmoType ammoType; // Tipo di munizioni
    public int nAmmo = 0; // Numero di munizioni per caricatore
    public int maxAmmo = 0; // Numero massimo di munizioni
    public float damageAmmo = 0; // Danno delle munizioni
    public int ammoToAdd = 0; // Munizioni da aggiungere

    public enum AmmoType
    {
        None,          // Nessuna munizione (per armi da mischia)
        PistolAmmo,    // Munizioni per pistole
        ShotgunAmmo,   // Munizioni per fucili a pompa
        AssaultRifleAmmo, // Munizioni per fucili d'assalto
        SniperAmmo // Munizioni per fucili da cecchino
    }

    public void Initialize(AmmoData ammoData)
    {
        nameItem = ammoData.nameItem;
        description = ammoData.description;
        tagType = ammoData.tagType;
        value = ammoData.value;
        valueSanita = ammoData.valueSanita;
        effectType = ammoData.effectType;
        weight = ammoData.weight;
        ingredientsRecipe = ammoData.ingredientsRecipe;
        qtaIngredientsRecipe = ammoData.qtaIngredientsRecipe;
        craftItem = !string.IsNullOrEmpty(ammoData.craftItem) ? ItemManager.Instance.GetItemByName(ammoData.craftItem) : null;
        isUsable = ammoData.isUsable;
        isShooting = ammoData.isShooting;
        isPickUp = ammoData.isPickUp;
        canDestroy = ammoData.canDestroy;
        isInCraft = ammoData.isInCraft;
        isStackable = ammoData.isStackable;
        inventorySectionType = ammoData.inventorySectionType;
        own = ammoData.own;
        qta = ammoData.qta;
        var sprites = SpriteManager.Instance.GetSprites(nameItem);
        icon = sprites.icon;
        image = sprites.image;
        ammoType = ammoData.ammoType;
        nAmmo = ammoData.nAmmo;
        maxAmmo = ammoData.maxAmmo;
        damageAmmo = ammoData.damageAmmo;
        ammoToAdd = ammoData.ammoToAdd;
    }
}
