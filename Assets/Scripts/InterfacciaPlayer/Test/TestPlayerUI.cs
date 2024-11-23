using UnityEngine;

public class TestPlayerUI : MonoBehaviour
{
    public PlayerUIController playerUIController; // Riferimento al componente PlayerUIController
    private int maxHealth = 100; // Salute massima
    private int currentHealth; // Salute attuale

    private void Start()
    {
        currentHealth = maxHealth; // Inizializza la salute attuale
        // playerUIController.UpdateAmmoCount(currentAmmo); // Inizializza il conteggio delle munizioni
        playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Inizializza l'immagine del sangue
        playerUIController.UpdateSanityIcon(); // Inizializza l'icona della sanità

        Debug.Log("Inizio del gioco: Salute = " + currentHealth);
    }

    private void Update()
    {
        // Aumenta la salute
        if (Input.GetKeyDown(KeyCode.H))
        {
            currentHealth += 10; // Aumenta la salute di 10
            currentHealth = Mathf.Min(currentHealth, maxHealth); // Assicurati che la salute non superi il massimo
            playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Aggiorna l'immagine del sangue
            playerUIController.UpdateSanityIcon(); // Aggiorna l'icona della sanità
            Debug.Log("Salute aumentata: " + currentHealth);
        }

        // Diminuisci la salute
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentHealth -= 10; // Diminuisce la salute di 10
            currentHealth = Mathf.Max(currentHealth, 0); // Assicurati che la salute non scenda sotto zero
            playerUIController.UpdateBloodSplatter(currentHealth, maxHealth); // Aggiorna l'immagine del sangue
            playerUIController.UpdateSanityIcon(); // Aggiorna l'icona della sanità
            Debug.Log("Salute diminuita: " + currentHealth);
        }
    }
}
