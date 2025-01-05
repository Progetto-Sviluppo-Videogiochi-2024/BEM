using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Create New Weapon")]
public class Weapon : Item // Weapon estende Item
{
    [Header("Info Weapon")]
    public GameObject prefab; // Prefab dell'arma
    public WeaponType weaponType; // Tipo di arma (da mischia, da fuoco)
    public RangeType rangeType; // Tipo di raggio (corto, medio)
    public Ammo ammo; // Munizioni dell'arma (se applicabile)
    public float distance; // Distanza massima di tiro dell'arma
    public bool semiAuto; // Se l'arma è semiautomatica (cioè spara un colpo per volta)
    public bool isThrowable; // Se l'arma è da lancio (coltelli, bottiglie)

    [Header("Audio Properties")]
    public AudioClip fireSound; // Suono di sparo
    public AudioClip magInSound; // Suono di inserimento del caricatore
    public AudioClip magOutSound; // Suono di estrazione del caricatore
    public AudioClip releaseSlideSound; // Suono di rilascio del caricatore

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

    public void Initialize(WeaponData itemData)
    {
        base.Initialize(itemData);
        if (itemData is not WeaponData weaponData) return;
        prefab = Resources.Load<GameObject>(weaponData.prefabName);
        weaponType = weaponData.weaponType;
        rangeType = weaponData.rangeType;
        ammo = Resources.Load<Ammo>(weaponData.ammoName);
        distance = weaponData.distance;
        semiAuto = weaponData.semiAuto;
        isThrowable = weaponData.isThrowable;
        fireSound = Resources.Load<AudioClip>(weaponData.fireSoundPath);
        magInSound = Resources.Load<AudioClip>(weaponData.magInSoundPath);
        magOutSound = Resources.Load<AudioClip>(weaponData.magOutSoundPath);
        releaseSlideSound = Resources.Load<AudioClip>(weaponData.releaseSlideSoundPath);
        IdlePosition = weaponData.IdlePosition;
        IdleRotation = weaponData.IdleRotation;
        AimPosition = weaponData.AimPosition;
        AimRotation = weaponData.AimRotation;
        Scale = weaponData.Scale;
    }
}
