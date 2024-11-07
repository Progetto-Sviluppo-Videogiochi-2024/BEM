using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Create New Weapon")]
public class Weapon : Item // Weapon estende Item
{
    [Header("Info Weapon")]
    public WeaponType weaponType; // Tipo di arma (da mischia, da fuoco)
    public RangeType rangeType; // Tipo di raggio (corto, medio)
    public Ammo ammo; // Munizioni dell'arma (se applicabile)

    public bool isThrowable; // Se l'arma è da lancio (coltelli, bottiglie)
    
    [Header("Positioning Weapon in Hands")]
    public Vector3 equippedPosition; // Posizione dell'arma equipaggiata
    public Quaternion equippedRotation; // Rotazione dell'arma equipaggiata
    public Vector3 equippedScale; // Scala dell'arma equipaggiata

    public Vector3 refRightHandGripPosition; // Posizione della mano destra per l'impugnatura
    public Quaternion refRightHandGripRotation; // Rotazione della mano destra per l'impugnatura
    public Vector3 refLeftHandGripPosition; // Posizione della mano sinistra per l'impugnatura
    public Quaternion refLeftHandGripRotation; // Rotazione della mano sinistra per l'impugnatura

    public enum WeaponType
    {
        Melee,      // Armi da mischia (coltelli, asce, mazze)
        Ranged     // Armi da fuoco (pistole, fucili)
    }

    public enum RangeType
    {
        ShortRange,  // Corto raggio (pistole, fucili a pompa)
        MediumRange  // Medio raggio (fucili d'assalto)
    }
}
