using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour 
{
    public Image weaponImage;
    public TextMeshProUGUI ammoText;
    public Image bloodSplatterImage;
    public Image sanityIcon; // Facoltativo

    public Sprite assaultRifleSprite;
    public Sprite shotgunSprite;
    public Sprite pistolSprite;
    public Sprite pistolHSprite;
    public Sprite huntingRifleSprite;

    // Variabile per tenere traccia dello stato dell'arma equipaggiata
    private bool isWeaponEquipped = false;

    // Nuova variabile per le munizioni massime
    private int maxAmmo = 0;

    public int MaxAmmo // Aggiungi questa proprietà
    {
        get { return maxAmmo; }
    }

    public void UpdateWeaponImage(string weaponName)
    {
        switch (weaponName)
        {
            case "AssaultRifle":
                weaponImage.sprite = assaultRifleSprite;
                maxAmmo = 40; // Imposta munizioni massime per l'arma
                break;
            case "Shotgun":
                weaponImage.sprite = shotgunSprite;
                maxAmmo = 8; // Imposta munizioni massime per l'arma
                break;
            case "Pistol":
                weaponImage.sprite = pistolSprite;
                maxAmmo = 15; // Imposta munizioni massime per l'arma
                break;
            case "PistolH":
                weaponImage.sprite = pistolHSprite;
                maxAmmo = 10; // Imposta munizioni massime per l'arma
                break;
            case "HuntingRifle":
                weaponImage.sprite = huntingRifleSprite;
                maxAmmo = 5; // Imposta munizioni massime per l'arma
                break;
            default:
                weaponImage.sprite = null;
                maxAmmo = 0; // Resetta munizioni massime se l'arma non è equipaggiata
                break;
        }

        // Mostra o nasconde l'immagine dell'arma in base alla sua equipaggiatura
        isWeaponEquipped = weaponImage.sprite != null;
        weaponImage.enabled = isWeaponEquipped; // Attiva/disattiva l'immagine
        ammoText.enabled = isWeaponEquipped; // Attiva/disattiva il testo delle munizioni
    }


    public void UpdateAmmoCount(int currentAmmo)
    {
        ammoText.text = $"{currentAmmo}/{maxAmmo}"; // Mostra le munizioni nel formato "10/40"
    }

    public void UpdateBloodSplatter(int health, int maxHealth)
    {
        // Mostra l'immagine del sangue e aumenta l'opacità in base ai danni subiti
        bloodSplatterImage.enabled = health < maxHealth; // Attiva l'immagine se la salute è diminuita
        float alpha = 1 - ((float)health / maxHealth);
        Color color = bloodSplatterImage.color;
        color.a = alpha; // Maggiore è il danno, più alta è l'opacità
        bloodSplatterImage.color = color;
    }

    public void UpdateSanityIcon(int health, int maxHealth)
    {
        // Mostra l'icona della sanità solo se la salute è inferiore o uguale alla metà
        sanityIcon.enabled = health <= maxHealth / 2;
    }
}
