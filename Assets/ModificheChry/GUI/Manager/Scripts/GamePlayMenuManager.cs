using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gamePlayMenuCanvas;
    public Button buttonCheckpoint;
    public Button buttonLoadGame;
    public Button buttonOptions;
    public Button buttonResumeGame;

    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool isMenuOpen = false;
    #endregion

    [Header("References")]
    #region References
    private Transform player;
    private Diario diario;
    [SerializeField] private SaveSlot saveSlot;
    [SerializeField] private LoadSlot loadSlot;
    #endregion

    void Start()
    {
        diario = FindObjectOfType<Diario>();
        player = FindAnyObjectByType<Player>()?.transform;

        gamePlayMenuCanvas.SetActive(false);
        buttonCheckpoint.onClick.AddListener(ReloadLastCheckpoint);
        buttonLoadGame.onClick.AddListener(LoadGame);
        buttonOptions.onClick.AddListener(OpenOptions);
        buttonResumeGame.onClick.AddListener(ReturnToGame);
    }

    void Update()
    {
        if (!saveSlot.loadSavePanel.activeSelf && !loadSlot.loadSavePanel.activeSelf && Input.GetKeyDown(KeyCode.Escape)) ToggleMenu(!isMenuOpen);
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
        // Pickup degli item già negato in ItemPickup
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
    private void ResumeGame() => ToggleMenu(false);

    public void ReturnToMainMenu() // Passato dall'inspector al "sì" del pannello di conferma
    {
        ResumeGame();
        BooleanAccessor.istance.ResetBoolValues();
        SceneManager.LoadScene("MainMenu");
    }

    private void ReloadLastCheckpoint()
    {
        print("Riprendi dall'ultimo Checkpoint");
        ResumeGame(); // Chiude il menu e riprende il gioco
        // TODO: da implementare il caricamento dell'ultimo checkpoint
    }

    private void ReturnToGame() => ResumeGame(); // Chiude il menu e riprende il gioco

    private void LoadGame()
    {
        ResumeGame(); // Chiude il menu e riprende il gioco
        // Passata una funzione come listener al pulsante di carica game per aprire quel canvas quindi non serve farlo da codice
    }

    private void OpenOptions()
    {
        print("Apri Opzioni");
        ResumeGame(); // Chiude il menu principale per aprire le opzioni
        // TODO: da implementare l'apertura del menu delle opzioni
    }
}
