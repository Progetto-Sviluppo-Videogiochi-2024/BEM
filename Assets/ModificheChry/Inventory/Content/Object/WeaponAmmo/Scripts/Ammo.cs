using System;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Create New Ammo")]
public class Ammo : Item
{
    [Header("Info Ammo")]
    public GameObject prefab; // Prefab delle munizioni
    public AmmoType ammoType; // Tipo di munizioni
    public int nAmmo = 0; // Numero di munizioni per caricatore
    public int maxAmmo = 0; // Numero massimo di munizioni
    public float damageAmmo = 0; // Danno delle munizioni

    public enum AmmoType
    {
        None,          // Nessuna munizione (per armi da mischia)
        PistolAmmo,    // Munizioni per pistole
        ShotgunAmmo,   // Munizioni per fucili a pompa
        AssaultRifleAmmo, // Munizioni per fucili d'assalto
        SniperAmmo // Munizioni per fucili da cecchino
    }

    public void Initialize(AmmoData itemData)
    {
        base.Initialize(itemData);
        if (itemData is not AmmoData ammoData) return;
        prefab = ammoData.prefab;
        ammoType = ammoData.ammoType;
        nAmmo = ammoData.nAmmo;
        maxAmmo = ammoData.maxAmmo;
        damageAmmo = ammoData.damageAmmo;
    }

    private int GetAmmoAmountFromDescription()
    {
        // Usa una regex per catturare il primo numero nella stringa
        Match match = Regex.Match(description, @"(\d+)\s+munizioni");
        if (match.Success) return int.Parse(match.Groups[1].Value);

        // Se nessun numero viene trovato, ritorna 0
        throw new ArgumentException("Description not formatted correctly for ammo pickup: " + description);
    }
}
