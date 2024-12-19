using DialogueEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    #region UI Elements
    public Transform gameOverMenuCanvas;
    #endregion

    [Header("References")]
    #region References
    private Transform player;
    private Diario diario;
    #endregion

    void Start()
    {
        player = FindObjectOfType<Player>()?.transform;
        diario = FindObjectOfType<Diario>();

        gameOverMenuCanvas.gameObject.SetActive(false);
        var panel = gameOverMenuCanvas.GetChild(0);
        panel.GetChild(2).GetComponent<Button>().onClick.AddListener(() => ReloadLastCheckpoint());
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

    private void ReloadLastCheckpoint()
    {
        ToggleMenu(false);
        // TODO: Da implementare il caricamento dell'ultimo checkpoint
    }

    private void LoadGame()
    {
        ToggleMenu(false);
        // Load game passata dall'inspector
    }
}
