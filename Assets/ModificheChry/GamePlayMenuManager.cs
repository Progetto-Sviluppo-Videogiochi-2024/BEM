using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gamePlayMenuCanvas;
    //public Button buttonMainMenu;
    public Button buttonCheckpoint;
    public Button buttonLoadGame;
    public Button buttonOptions;
    //public Button buttonQuit;

    [Header("Settings")]
    #region Settings
    private bool isMenuOpen = false;
    #endregion

    [Header("References")]
    #region References
    private Transform player;
    #endregion

    void Start()
    {
        if (FindObjectOfType<Player>() != null) player = FindAnyObjectByType<Player>().transform;

        // Assicura che il menu sia inizialmente nascosto
        gamePlayMenuCanvas.SetActive(false);

        // Configura i listener per i pulsanti
        buttonCheckpoint.onClick.AddListener(ReloadLastCheckpoint);
        buttonLoadGame.onClick.AddListener(LoadGame);
        buttonOptions.onClick.AddListener(OpenOptions);
        //buttonMainMenu.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if (isMenuOpen) ToggleCursor(true);
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenu(!isMenuOpen);
        }
    }

    private void ToggleMenu(bool isOpen)
    {
        isMenuOpen = isOpen;
        gamePlayMenuCanvas.SetActive(isMenuOpen);
        ToggleCursor(isMenuOpen);

        if (player != null) player.GetComponent<AimStateManager>().enabled = !isMenuOpen; // Per la visuale

        Time.timeScale = isMenuOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
    }

    private void ToggleCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    // Metodo per ripristinare Time.timeScale e chiudere il menu
    private void ResumeGame()
    {
        isMenuOpen = false;
        gamePlayMenuCanvas.SetActive(false);
        Time.timeScale = 1; // Riprendi il tempo di gioco
    }

    public void ReturnToMainMenu() // Passato dall'inspector quando fa "sì" al pannello di conferma
    {
        ResumeGame();
        BooleanAccessor.istance.ResetBoolValues(); // Reset dei valori
        SceneManager.LoadScene("MainMenu");
    }

    private void ReloadLastCheckpoint()
    {
        Debug.Log("Riprendi dall'ultimo Checkpoint");
        ResumeGame(); // Chiude il menu e riprende il gioco
    }

    private void LoadGame()
    {
        Debug.Log("Carica Partita Salvata");
        ResumeGame(); // Chiude il menu e riprende il gioco
    }

    private void OpenOptions()
    {
        Debug.Log("Apri Opzioni");
        ResumeGame(); // Chiude il menu principale per aprire le opzioni
    }
}
