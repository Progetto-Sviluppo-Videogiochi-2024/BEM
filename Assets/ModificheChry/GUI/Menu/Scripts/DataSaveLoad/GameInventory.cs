using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInventory : MonoBehaviour, IBind<InventoryData>
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // ID univoco dell'inventario
    [SerializeField] public InventoryData data; // Dati dell'inventario

    public void Bind(InventoryData data) // Carica da file
    {
        this.data = data;
        this.data.Id = Id;

        if (TryGetComponent(out InventoryManager inventory))
        {
            inventory.ClearInventory(); // Pulisce l'inventario prima di caricare i dati

            IterateOnList<Item, ItemData>(data.items, inventory);
            IterateOnList<Weapon, WeaponData>(data.weaponItems, inventory);
            IterateOnList<Ammo, AmmoData>(data.ammoItems, inventory);
        }

        WeaponAmmo.collectedAmmoTypes = new(data.ammoIdCollected);
    }

    private void IterateOnList<T, TData>(List<TData> datas, InventoryManager inventory) where T : Item, new()
    {
        foreach (TData data in datas)
        {
            T itemInstance = inventory.CreateInstance<T, TData>(data);
            inventory.Add(itemInstance);
            if (data is ItemData itemData) itemInstance.qta = itemData.qta;
            else if (data is WeaponData weaponData) itemInstance.qta = weaponData.qta;
            else if (data is AmmoData ammoData) itemInstance.qta = ammoData.qta;
        }
    }

    public void SaveInventoryData() // Salva su file
    {
        if (TryGetComponent(out InventoryManager inventory))
        {
            data.items.Clear(); // Pulisce la lista degli oggetti salvati
            data.weaponItems.Clear(); // Pulisce la lista delle armi salvate
            data.ammoItems.Clear(); // Pulisce la lista delle munizioni salvate
            data.ammoIdCollected.Clear(); // Pulisce la lista delle munizioni raccolte

            // Salva ogni oggetto dell'inventario
            foreach (var item in inventory.items)
            {
                if (item is Weapon weapon)
                {
                    var weaponData = new WeaponData(weapon);
                    data.weaponItems.Add(weaponData);
                }
                else if (item is Ammo ammo)
                {
                    var ammoData = new AmmoData(ammo);
                    data.ammoItems.Add(ammoData);
                }
                else
                {
                    var itemData = new ItemData(item);
                    data.items.Add(itemData);
                }
            }

            foreach (string ammo in WeaponAmmo.collectedAmmoTypes)
            {
                data.ammoIdCollected.Add(ammo);
            }
        }
    }
}

[Serializable]
public class InventoryData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid(); // ID univoco dell'inventario
    public List<ItemData> items = new(); // Lista degli oggetti nell'inventario
    public List<WeaponData> weaponItems = new(); // Lista delle armi nell'inventario
    public List<AmmoData> ammoItems = new(); // Lista delle munizioni nell'inventario
    public List<string> ammoIdCollected = new(); // Lista delle munizioni raccolte, per evitare di configurare più volte la stessa ammo raccolta
}

[Serializable]
public class ItemData
{
    public string type; // Identifica il tipo ("Item", "Weapon", "Ammo")
    public string nameItem;
    public string description;
    public Item.ItemTagType tagType;
    public int value;
    public int valueSanita;
    public Item.ItemEffectType effectType;
    public float weight;
    public string ingredientsRecipe;
    public string qtaIngredientsRecipe;
    public string craftItem;
    public bool isUsable;
    public bool isShooting;
    public bool isPickUp;
    public bool canDestroy;
    public bool isInCraft;
    public bool isStackable;
    public Item.ItemType inventorySectionType;
    public bool own;
    public int qta;
    // public Sprite icon; // La prendo già da SpriteManager
    // public Sprite image; // La prendo già da SpriteManager

    public ItemData() { }

    public ItemData(Item item)
    {
        type = "Item";
        nameItem = item.nameItem;
        description = item.description;
        tagType = item.tagType;
        value = item.value;
        valueSanita = item.valueSanita;
        effectType = item.effectType;
        weight = item.weight;
        ingredientsRecipe = item.ingredientsRecipe;
        qtaIngredientsRecipe = item.qtaIngredientsRecipe;
        craftItem = item.craftItem?.nameItem;
        isUsable = item.isUsable;
        isShooting = item.isShooting;
        isPickUp = item.isPickUp;
        canDestroy = item.canDestroy;
        isInCraft = item.isInCraft;
        isStackable = item.isStackable;
        inventorySectionType = item.inventorySectionType;
        own = item.own;
        qta = item.qta;
    }
}

[Serializable]
public class WeaponData
{
    public string type;
    public string nameItem;
    public string description;
    public Item.ItemTagType tagType;
    public int value;
    public int valueSanita;
    public Item.ItemEffectType effectType;
    public float weight;
    public string ingredientsRecipe;
    public string qtaIngredientsRecipe;
    public string craftItem;
    public bool isUsable;
    public bool isShooting;
    public bool isPickUp;
    public bool canDestroy;
    public bool isInCraft;
    public bool isStackable;
    public Item.ItemType inventorySectionType;
    public bool own;
    public int qta;
    // public Sprite icon; // La prendo già da SpriteManager
    // public Sprite image; // La prendo già da SpriteManager
    public string prefabName; // Non la uso perché lo istanzio in IM nel bind dei dati
    public Weapon.WeaponType weaponType;
    public Weapon.RangeType rangeType;
    public string ammoName;
    public float distance;
    public bool semiAuto;
    public bool isThrowable;
    public Vector3 IdlePosition;
    public Quaternion IdleRotation;
    public Vector3 AimPosition;
    public Quaternion AimRotation;
    public Vector3 Scale;
    public bool isLoadingSlot;
    public int bulletConsumed;
    public int currentAmmo;
    public int extraAmmo;

    public WeaponData() { }

    public WeaponData(Weapon weapon)
    {
        type = "Weapon";
        nameItem = weapon.nameItem;
        description = weapon.description;
        tagType = weapon.tagType;
        value = weapon.value;
        valueSanita = weapon.valueSanita;
        effectType = weapon.effectType;
        weight = weapon.weight;
        ingredientsRecipe = weapon.ingredientsRecipe;
        qtaIngredientsRecipe = weapon.qtaIngredientsRecipe;
        craftItem = weapon.craftItem?.nameItem;
        isUsable = weapon.isUsable;
        isShooting = weapon.isShooting;
        isPickUp = weapon.isPickUp;
        canDestroy = weapon.canDestroy;
        isInCraft = weapon.isInCraft;
        isStackable = weapon.isStackable;
        inventorySectionType = weapon.inventorySectionType;
        own = weapon.own;
        qta = weapon.qta;
        prefabName = weapon.prefab.name;
        weaponType = weapon.weaponType;
        rangeType = weapon.rangeType;
        ammoName = weapon.ammo.nameItem;
        distance = weapon.distance;
        semiAuto = weapon.semiAuto;
        isThrowable = weapon.isThrowable;
        IdlePosition = weapon.IdlePosition;
        IdleRotation = weapon.IdleRotation;
        AimPosition = weapon.AimPosition;
        AimRotation = weapon.AimRotation;
        Scale = weapon.Scale;
        isLoadingSlot = false;
        bulletConsumed = weapon.prefab.GetComponent<WeaponManager>().bulletConsumed;
        var weaponAmmo = weapon.prefab.GetComponent<WeaponAmmo>();
        currentAmmo = weaponAmmo.currentAmmo;
        extraAmmo = weaponAmmo.extraAmmo;
    }
}

[Serializable]
public class AmmoData
{
    public string type;
    public string nameItem;
    public string description;
    public Item.ItemTagType tagType;
    public int value;
    public int valueSanita;
    public Item.ItemEffectType effectType;
    public float weight;
    public string ingredientsRecipe;
    public string qtaIngredientsRecipe;
    public string craftItem;
    public bool isUsable;
    public bool isShooting;
    public bool isPickUp;
    public bool canDestroy;
    public bool isInCraft;
    public bool isStackable;
    public Item.ItemType inventorySectionType;
    public bool own;
    public int qta;
    // public Sprite icon; // La prendo già da SpriteManager
    // public Sprite image; // La prendo già da SpriteManager
    public Ammo.AmmoType ammoType;
    public int nAmmo;
    public int maxAmmo;
    public float damageAmmo;
    public int ammoToAdd;
    // public GameObject prefab; // Lo passo già a WeaponManager

    public AmmoData() { }

    public AmmoData(Ammo ammo)
    {
        type = "Ammo";
        nameItem = ammo.nameItem;
        description = ammo.description;
        tagType = ammo.tagType;
        value = ammo.value;
        valueSanita = ammo.valueSanita;
        effectType = ammo.effectType;
        weight = ammo.weight;
        ingredientsRecipe = ammo.ingredientsRecipe;
        qtaIngredientsRecipe = ammo.qtaIngredientsRecipe;
        craftItem = ammo.craftItem?.nameItem;
        isUsable = ammo.isUsable;
        isShooting = ammo.isShooting;
        isPickUp = ammo.isPickUp;
        canDestroy = ammo.canDestroy;
        isInCraft = ammo.isInCraft;
        isStackable = ammo.isStackable;
        inventorySectionType = ammo.inventorySectionType;
        own = ammo.own;
        qta = ammo.qta;
        ammoType = ammo.ammoType;
        nAmmo = ammo.nAmmo;
        maxAmmo = ammo.maxAmmo;
        damageAmmo = ammo.damageAmmo;
        ammoToAdd = ammo.ammoToAdd;
    }
}
