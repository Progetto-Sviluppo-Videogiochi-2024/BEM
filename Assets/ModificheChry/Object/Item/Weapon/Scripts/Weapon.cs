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
    public Vector3 IdlePosition; // Posizione dell'arma equipaggiata
    public Quaternion IdleRotation; // Rotazione dell'arma equipaggiata
     public Vector3 AimPosition; // Posizione dell'arma equipaggiata
    public Quaternion AimRotation; // Rotazione dell'arma equipaggiata
    public Vector3 Scale; // Scala dell'arma equipaggiata
    
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
