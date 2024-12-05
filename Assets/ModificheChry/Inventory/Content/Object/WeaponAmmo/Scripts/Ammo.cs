using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Create New Ammo")]
public class Ammo : Item
{
    [Header("Info Ammo")]
    public GameObject ammoPrefab; // Prefab delle munizioni
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
}
