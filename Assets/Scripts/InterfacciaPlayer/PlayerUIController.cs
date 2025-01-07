using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUIController : MonoBehaviour
{
    [Header("Player UI")]
    #region Player UI
    [SerializeField] private Image weaponImage; // Immagine dell'arma equipaggiata
    [SerializeField] private TextMeshProUGUI ammoText; // Testo per le munizioni rimanenti dell'arma equipaggiata
    [SerializeField] private Image bloodSplatterImage; // Immagine del sangue sullo schermo
    [SerializeField] private Image sanityIcon; // Icona della sanità mentale del giocatore
    [SerializeField] private Image itemIcon; // Icona dell'oggetto equipaggiato
    #endregion

    [Header("References")]
    #region References
    public Player player; // Riferimento al componente Player
    [HideInInspector] public int extraAmmo; // Munizioni extra per l'arma equipaggiata
    #endregion

    public void UpdateItemUI() // TODO: da testare
    {
        var currentItem = InventoryManager.instance.itemEquipable;
        if (currentItem == null) { itemIcon.enabled = false; return; }
        itemIcon.sprite = currentItem.icon;
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

    public void UpdateAmmoCount(int currentAmmo) => ammoText.text = $"{currentAmmo} / {extraAmmo}"; // Formato "left (nel caricatore) / extra"

    public void UpdateBloodSplatter(int health, int maxHealth)
    {
        // Mostra l'immagine del sangue e aumenta l'opacità in base ai danni subiti
        bloodSplatterImage.enabled = player.health < player.maxHealth; // Attiva l'immagine se la salute è diminuita
        float alpha = 1 - ((float)health / maxHealth);
        Color color = bloodSplatterImage.color;
        color.a = alpha; // Maggiore è il danno, più alta è l'opacità
        bloodSplatterImage.color = color;
    }

    public void UpdateSanityIcon(int sanitaMentale, int maxHealth)
    {
        // Mostra l'icona della sanità mentale e aumenta l'opacità in base ai danni subiti
        sanityIcon.enabled = sanitaMentale < maxHealth; // Attiva l'icona se la sanità mentale è diminuita
        float alpha = 1 - ((float)sanitaMentale / maxHealth);
        Color color = sanityIcon.color;
        color.a = alpha; // Maggiore è il danno, più alta è l'opacità
        sanityIcon.color = color;
    }
}
