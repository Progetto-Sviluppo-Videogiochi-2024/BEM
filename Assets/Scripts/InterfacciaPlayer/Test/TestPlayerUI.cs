using UnityEngine;

public class TestPlayerUI : MonoBehaviour
{
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController

    private int currentAmmo = 40;  // Munizioni iniziali per l'arma equipaggiata
    private int maxHealth = 100;    // Salute massima
    private int currentHealth;       // Salute attuale

    private void Start()
    {
        currentHealth = maxHealth; // Inizializza la salute attuale
        playerUIController.UpdateAmmoCount(currentAmmo); // Inizializza il conteggio delle munizioni
        playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Inizializza l'immagine del sangue
        playerUIController.UpdateSanityIcon(currentHealth, maxHealth); // Inizializza l'icona della sanità

        Debug.Log("Inizio del gioco: Salute = " + currentHealth + ", Munizioni = " + currentAmmo);
    }

    private void Update()
    {
        // Cambia arma
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            playerUIController.UpdateWeaponImage("AssaultRifle");
            Debug.Log("Arma equipaggiata: Assault Rifle");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            playerUIController.UpdateWeaponImage("Shotgun");
            Debug.Log("Arma equipaggiata: Shotgun");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            playerUIController.UpdateWeaponImage("Pistol");
            Debug.Log("Arma equipaggiata: Pistol");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            playerUIController.UpdateWeaponImage("PistolH");
            Debug.Log("Arma equipaggiata: Pistol H");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            playerUIController.UpdateWeaponImage("HuntingRifle");
            Debug.Log("Arma equipaggiata: Hunting Rifle");
        }

        // Consuma munizioni
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentAmmo -= 2; // Consuma 2 colpi
            currentAmmo = Mathf.Max(currentAmmo, 0); // Assicurati che le munizioni non scendano sotto zero
            playerUIController.UpdateAmmoCount(currentAmmo);
            Debug.Log("Munizioni consumate: " + currentAmmo + "/" + playerUIController.MaxAmmo);
        }

        // Aumenta la salute
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHealth += 10; // Aumenta la salute di 10
            currentHealth = Mathf.Min(currentHealth, maxHealth); // Assicurati che la salute non superi il massimo
            playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Aggiorna l'immagine del sangue
            playerUIController.UpdateSanityIcon(currentHealth, maxHealth); // Aggiorna l'icona della sanità
            Debug.Log("Salute aumentata: " + currentHealth);
        }

        // Diminuisci la salute
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentHealth -= 10; // Diminuisce la salute di 10
            currentHealth = Mathf.Max(currentHealth, 0); // Assicurati che la salute non scenda sotto zero
            playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Aggiorna l'immagine del sangue
            playerUIController.UpdateSanityIcon(currentHealth, maxHealth); // Aggiorna l'icona della sanità
            Debug.Log("Salute diminuita: " + currentHealth);
        }
    }
}
