using DialogueEditor;
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
    public Button buttonResumeGame;
    //public Button buttonQuit;

    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool isMenuOpen = false;
    #endregion

    [Header("References")]
    #region References
    private Transform player;
    private Diario diario;
    #endregion

    void Start()
    {
        diario = FindObjectOfType<Diario>();
        player = FindAnyObjectByType<Player>()?.transform;

        // Assicura che il menu sia inizialmente nascosto
        gamePlayMenuCanvas.SetActive(false);

        // Configura i listener per i pulsanti
        buttonCheckpoint.onClick.AddListener(ReloadLastCheckpoint);
        buttonLoadGame.onClick.AddListener(LoadGame);
        buttonOptions.onClick.AddListener(OpenOptions);
        buttonResumeGame.onClick.AddListener(ReturnToGame);
        //buttonMainMenu.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        // if (isMenuOpen) ToggleScripts(true);

        if (Input.GetKeyDown(KeyCode.P))
        {
            ToggleMenu(!isMenuOpen);
        }
    }

    private void ToggleMenu(bool isOpen)
    {
        isMenuOpen = isOpen;
        gamePlayMenuCanvas.SetActive(isMenuOpen);
        GestoreScena.ChangeCursorActiveStatus(isMenuOpen, "gamePlayMenu");
        ToggleScripts(isMenuOpen);

        Time.timeScale = isMenuOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
    }

    private void ToggleScripts(bool visible)
    {
        if (player != null)
        {
            player.GetComponent<ActionStateManager>().enabled = !visible; // Per le azioni (ricarica e switch dell'arma, ecc.)
            player.GetComponent<WeaponClassManager>().enabled = !visible; // Per le armi
            player.GetComponent<OpenInventory>().enabled = !visible; // Per l'inventario
            player.GetComponent<AimStateManager>().enabled = !visible; // Per la visuale
        }
        if (diario != null) diario.enabled = !visible; // Per il diario
        ConversationManager.Instance.enabled = !visible; // Per le conversazioni
    }

    // Metodo per ripristinare Time.timeScale e chiudere il menu
    private void ResumeGame()
    {
        ToggleMenu(false);        
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

    private void ReturnToGame()
    {
        Debug.Log("Riprendi la scena corrente");
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
