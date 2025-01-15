using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gamePlayMenuCanvas; // Canvas del menu di gioco
    private Button buttonCheckpoint; // Button per caricare l'ultimo checkpoint
    private Button buttonOptions; // Button per aprire le opzioni
    private Button buttonResumeGame; // Button per riprendere il gioco
    [SerializeField] private GameObject confirmCheckpoint; // Pannello di conferma del caricamento del checkpoint

    [Header("Settings")]
    #region Settings
    [HideInInspector] public bool isMenuOpen = false; // Flag per il menu di gioco aperto
    #endregion

    [Header("References")]
    #region References
    private Transform player; // Riferimento al giocatore
    private Diario diario; // Riferimento al diario
    [SerializeField] private SaveSlot saveSlot; // Riferimento al SaveSlot
    [SerializeField] private LoadSlot loadSlot; // Riferimento al LoadSlot
    [SerializeField] private GameObject options; // Pannello delle opzioni
    [SerializeField] private GameObject confirmPanel; // Pannello di conferma
    #endregion

    void Start()
    {
        diario = FindObjectOfType<Diario>();
        player = FindAnyObjectByType<Player>()?.transform;

        var panel = gamePlayMenuCanvas.transform.GetChild(0);
        var backGPM = options.transform.GetChild(0).GetChild(1).GetComponent<Button>();

        buttonCheckpoint = panel.GetChild(2).GetComponent<Button>();
        // buttonLoadGame = panel.GetChild(3).GetComponent<Button>();
        buttonOptions = panel.GetChild(4).GetComponent<Button>();
        buttonResumeGame = panel.GetChild(5).GetComponent<Button>();

        gamePlayMenuCanvas.SetActive(false);
        buttonCheckpoint.onClick.AddListener(() => ReloadLastCheckpoint(buttonCheckpoint));
        // buttonLoadGame.onClick.AddListener(LoadGame); // Non necessaria
        buttonOptions.onClick.AddListener(OpenOptions);
        buttonResumeGame.onClick.AddListener(ReturnToGame);
        backGPM.onClick.AddListener(() => ToggleMenu(true));
    }

    void Update()
    {
        if (CanOpenMenu() && Input.GetKeyDown(KeyCode.Escape)) ToggleMenu(!isMenuOpen);
    }

    private bool CanOpenMenu() =>
        !(player?.GetComponent<Animator>()?.GetBool("sit") ?? false) && // Player non è seduto o è null
        !confirmPanel.activeSelf && // Se il pannello di conferma è attivo
        !saveSlot.loadSavePanel.activeSelf && // Se il pannello di salvataggio è attivo
        !loadSlot.loadSavePanel.activeSelf && // Se il pannello di caricamento è attivo
        !options.activeSelf; // Se options è attivo, allora non consenti di toggle il GPM

    public void ToggleMenu(bool isOpen)
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
            player.GetComponent<MovementStateManager>().enabled = !visible; // Per il movimento
            player.GetComponent<ActionStateManager>().enabled = !visible; // Per le azioni (ricarica e switch dell'arma, ecc.)
            player.GetComponent<WeaponClassManager>().enabled = !visible; // Per le armi
            player.GetComponent<OpenInventory>().enabled = !visible; // Per l'inventario
            player.GetComponent<AimStateManager>().enabled = !visible; // Per la visuale
        }
        if (diario != null) diario.enabled = !visible; // Per il diario
        ConversationManager.Instance.enabled = !visible; // Per le conversazioni

        var audiosources = player.GetComponents<AudioSource>();
        foreach (var audio in audiosources) audio.Stop();

        ToggleNearbyMutantAudio(!visible); // Per l'audio dei mutanti
    }

    private void ToggleNearbyMutantAudio(bool enable)
    {
        // Controlla se il nome della scena è "Scena3"
        if (SceneManager.GetActiveScene().name != "Scena3") return;

        float detectionRadius = 20f; // Raggio di rilevamento per i mutanti
        LayerMask mutantLayer = LayerMask.GetMask("Enemy"); // Assicurati che i mutanti siano in un layer specifico
        Collider[] nearbyObjects = Physics.OverlapSphere(player.transform.position, detectionRadius, mutantLayer);
        foreach (Collider obj in nearbyObjects)
        {
            AIAgent mutant = obj.GetComponent<AIAgent>();
            if (mutant != null && mutant.audioSource != null) mutant.audioSource.mute = !enable;
        }
    }

    private void ResumeGame() => ToggleMenu(false); // Metodo per ripristinare Time.timeScale e chiudere il menu

    public void ReturnToMainMenu() // Passato dall'inspector al "sì" del pannello di conferma
    {
        ResumeGame();
        BooleanAccessor.istance.ResetBoolValues();
        SceneManager.LoadScene("MainMenu");
    }

    private void ReloadLastCheckpoint(Button buttonCheckpoint)
    {
        buttonCheckpoint.interactable = SaveLoadSystem.Instance.dataService.DoesSaveExist("Checkpoint");
        if (!buttonCheckpoint.interactable) return;
        confirmCheckpoint.SetActive(true);
        ToggleScripts(true);
        gamePlayMenuCanvas.SetActive(false); // Non invoco ToggleMenu perché si toglie il cursore poi
    }

    public void OnYesConfirmCheckpoint() // Invocata dal button "sì" del pannello di conferma del checkpoint
    {
        confirmCheckpoint.SetActive(false);
        SaveLoadSystem.Instance.ReloadGame();
    }

    public void OnNoConfirmCheckpoint() // Invocata dal button "no" del pannello di conferma del checkpoint
    {
        confirmCheckpoint.SetActive(false);
        gamePlayMenuCanvas.SetActive(true);
        ToggleScripts(false);
    }

    private void ReturnToGame() => ResumeGame(); // Chiude il menu e riprende il gioco

    // private void LoadGame() // LG si apre già in quanto tale funzione è già passata al button dall'inspector

    private void OpenOptions()
    {
        // ResumeGame(); // Chiude il menu principale per aprire le opzioni
        options.SetActive(true);
        ToggleScripts(true);
        gamePlayMenuCanvas.SetActive(false); // Non invoco ToggleMenu perché si toglie il cursore poi
    }
}
