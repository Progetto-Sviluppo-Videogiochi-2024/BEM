using System;
using System.Collections.Generic;
using UnityEditor;
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

            // Ricrea gli oggetti dell'inventario in base ai dati salvati
            foreach (var itemData in data.items)
            {
                Item itemInstance = inventory.CreateItemInstance(itemData);
                inventory.Add(itemInstance);
                itemInstance.qta = itemData.qta;
            }
        }
    }

    public void SaveInventoryData() // Salva su file
    {
        if (TryGetComponent(out InventoryManager inventory))
        {
            data.items.Clear(); // Pulisce la lista degli oggetti salvati

            // Salva ogni oggetto dell'inventario
            foreach (var item in inventory.items)
            {
                if (item is Weapon weapon)
                {
                    var weaponData = new WeaponData(weapon);
                    data.items.Add(weaponData);
                }
                else if (item is Ammo ammo)
                {
                    var ammoData = new AmmoData(ammo);
                    data.items.Add(ammoData);
                }
                else
                {
                    var itemData = new ItemData(item);
                    data.items.Add(itemData);
                }
            }
        }
    }
}

[Serializable]
public class InventoryData : ISaveable
{
    [field: SerializeField] public SerializableGuid Id { get; set; } // ID univoco dell'inventario
    public List<ItemData> items = new(); // Lista degli oggetti nell'inventario
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
    public bool isUsable;
    public bool isShooting;
    public bool isPickUp;
    public bool canDestroy;
    public bool isInCraft;
    public bool isStackable;
    public Item.ItemType inventorySectionType;
    public bool own;
    public int qta;
    public Sprite icon;
    public Sprite image;

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
        isUsable = item.isUsable;
        isShooting = item.isShooting;
        isPickUp = item.isPickUp;
        canDestroy = item.canDestroy;
        isInCraft = item.isInCraft;
        isStackable = item.isStackable;
        inventorySectionType = item.inventorySectionType;
        own = item.own;
        qta = item.qta;
        // icon = item.icon; // La prendo già da SpriteManager
        // image = item.image; // La prendo già da SpriteManager
    }
}

[Serializable]
public class WeaponData : ItemData
{
    public string prefabName; // Salviamo il nome del prefab (non il GameObject)
    public Weapon.WeaponType weaponType;
    public Weapon.RangeType rangeType;
    public string ammoName; // Salviamo il nome dell'ammo associato (non il riferimento)
    public float distance;
    public bool semiAuto;
    public bool isThrowable;
    public string fireSoundPath;
    public string magInSoundPath;
    public string magOutSoundPath;
    public string releaseSlideSoundPath;
    public Vector3 IdlePosition;
    public Quaternion IdleRotation;
    public Vector3 AimPosition;
    public Quaternion AimRotation;
    public Vector3 Scale;

    public WeaponData(Weapon weapon) : base(weapon)
    {
        type = "Weapon";
        prefabName = weapon.prefab.name;
        weaponType = weapon.weaponType;
        rangeType = weapon.rangeType;
        ammoName = weapon.ammo ? weapon.ammo.nameItem : null;
        distance = weapon.distance;
        semiAuto = weapon.semiAuto;
        isThrowable = weapon.isThrowable;
#if UNITY_EDITOR
        fireSoundPath = AssetDatabase.GetAssetPath(weapon.fireSound);
        magInSoundPath = AssetDatabase.GetAssetPath(weapon.magInSound);
        magOutSoundPath = AssetDatabase.GetAssetPath(weapon.magOutSound);
        releaseSlideSoundPath = AssetDatabase.GetAssetPath(weapon.releaseSlideSound);
#endif

        IdlePosition = weapon.IdlePosition;
        IdleRotation = weapon.IdleRotation;
        AimPosition = weapon.AimPosition;
        AimRotation = weapon.AimRotation;
        Scale = weapon.Scale;
    }
}

[Serializable]
public class AmmoData : ItemData
{
    public GameObject prefab;
    public Ammo.AmmoType ammoType;
    public int nAmmo;
    public int maxAmmo;
    public float damageAmmo;

    public AmmoData(Ammo ammo) : base(ammo)
    {
        type = "Ammo";
        prefab = ammo.prefab;
        ammoType = ammo.ammoType;
        nAmmo = ammo.nAmmo;
        maxAmmo = ammo.maxAmmo;
        damageAmmo = ammo.damageAmmo;
    }
}
