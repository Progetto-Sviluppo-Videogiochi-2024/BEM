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

    private bool isMenuOpen = false;

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        gamePlayMenuCanvas.SetActive(isMenuOpen);

        // Mette in pausa il gioco quando il menu è aperto, lo riprende quando chiuso
        Time.timeScale = isMenuOpen ? 0 : 1;
    }

    // Metodo per ripristinare Time.timeScale e chiudere il menu
    private void ResumeGame()
    {
        isMenuOpen = false;
        gamePlayMenuCanvas.SetActive(false);
        Time.timeScale = 1; // Riprendi il tempo di gioco
    }

    public void ReturnToMainMenu() // Lo passo già dall'inspector quando fa "sì" al pannello di conferma
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
