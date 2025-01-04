using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    #region UI Elements
    public Transform gameOverMenuCanvas; // Canvas del menu di game over
    [SerializeField] private GameObject confirmCheckpoint; // Pannello di conferma del caricamento del checkpoint
    #endregion

    [Header("References")]
    #region References
    [SerializeField] private Transform player; // Riferimento al giocatore
    [SerializeField] private Diario diario; // Riferimento al diario
    #endregion

    void Start()
    {
        gameOverMenuCanvas.gameObject.SetActive(false);
        var panel = gameOverMenuCanvas.GetChild(0);
        var buttonReload = panel.GetChild(2).GetComponent<Button>();
        buttonReload.onClick.AddListener(() => ReloadLastCheckpoint(buttonReload));
    }

    public void ToggleMenu(bool isOpen)
    {
        gameOverMenuCanvas.gameObject.SetActive(isOpen);
        GestoreScena.ChangeCursorActiveStatus(isOpen, "gameOverMenu");
        ToggleScripts(isOpen);
        Time.timeScale = isOpen ? 0 : 1; // 0 = pausa, 1 = gioco normale
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
    }

    public void ReturnMM() // Passata dall'inspector al "sì" del pannello di conferma
    {
        ToggleMenu(false);
        BooleanAccessor.istance.ResetBoolValues();
        SceneManager.LoadScene("MainMenu");
    }

    private void ReloadLastCheckpoint(Button button)
    {
        if (!SaveLoadSystem.Instance.dataService.DoesSaveExist("Checkpoint"))
        {
            button.interactable = false;
            return;
        }

        confirmCheckpoint.SetActive(true);
        ToggleScripts(true);
        gameOverMenuCanvas.gameObject.SetActive(false); // Non invoco ToggleMenu perché si toglie il cursore poi
    }

    public void OnYesConfirmCheckpoint() // Invocata dal button "sì" del pannello di conferma del checkpoint
    {
        confirmCheckpoint.SetActive(false);
        SaveLoadSystem.Instance.ReloadGame();
    }

    public void OnNoConfirmCheckpoint() // Invocata dal button "no" del pannello di conferma del checkpoint
    {
        confirmCheckpoint.SetActive(false);
        gameOverMenuCanvas.gameObject.SetActive(true);
        ToggleScripts(false);
    }

    private void LoadGame()
    {
        ToggleMenu(false);
        // Load game passata dall'inspector
    }
}
