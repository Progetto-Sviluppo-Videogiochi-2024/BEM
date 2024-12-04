using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("Player UI")]
    #region Player UI
    private Image weaponImage; // Immagine dell'arma equipaggiata
    private TextMeshProUGUI ammoText; // Testo per le munizioni rimanenti dell'arma equipaggiata
    private Image bloodSplatterImage; // Immagine del sangue sullo schermo
    private Image sanityIcon; // Icona della sanità mentale del giocatore
    private Image itemIcon; // Icona dell'oggetto equipaggiato
    #endregion

    [Header("References")]
    #region References
    public Player player; // Riferimento al componente Player
    [HideInInspector] public int extraAmmo; // Munizioni extra per l'arma equipaggiata
    #endregion

    void Start()
    {
        weaponImage = transform.GetChild(0).GetComponent<Image>();
        ammoText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        bloodSplatterImage = transform.GetChild(2).GetComponent<Image>();
        sanityIcon = transform.GetChild(3).GetComponent<Image>();
        itemIcon = transform.GetChild(4).GetComponent<Image>();
    }

    public void UpdateItemUI() // TODO: da testare
    {
        var currentItem = InventoryManager.instance.itemEquipable;
        if (currentItem == null) { itemIcon.enabled = false; return; }
        itemIcon.sprite = currentItem.image;
        itemIcon.enabled = true;
    }

    public void UpdateWeaponUI()
    {
        var currentWeapon = player.weaponClassManager.currentWeapon;
        if (currentWeapon == null)
        {
            weaponImage.enabled = false;
            ammoText.enabled = false;
            return;
        }

        var weapon = currentWeapon.GetComponent<ItemController>().item as Weapon;
        weaponImage.sprite = weapon.image;
        weaponImage.enabled = true;
        ammoText.enabled = true;
    }

    public void UpdateAmmoCount(int currentAmmo) => ammoText.text = $"{currentAmmo} / {extraAmmo}"; // Formato "left / extra"

    public void UpdateBloodSplatter(int health, int maxHealth)
    {
        // Mostra l'immagine del sangue e aumenta l'opacità in base ai danni subiti
        bloodSplatterImage.enabled = player.health < player.maxHealth; // Attiva l'immagine se la salute è diminuita
        float alpha = 1 - ((float)health / maxHealth);
        Color color = bloodSplatterImage.color;
        color.a = alpha; // Maggiore è il danno, più alta è l'opacità
        bloodSplatterImage.color = color;
    }

    public void UpdateSanityIcon() => sanityIcon.enabled = player.health <= player.maxHealth / 2; // Icona visibile sse la salute è <= 50%
}
