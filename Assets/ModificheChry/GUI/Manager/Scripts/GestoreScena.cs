using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreScena : MonoBehaviour
{
    [Header("Next Scene")]
    #region Next Scene
    public string nextChapter = "Chapter I:"; // Nome del capitolo della scena successiva
    public string nextScene = "ScenaI"; // Nome della scena successiva
    #endregion

    [Header("Static References")]
    #region Static References
    private static int nUIOpen = 0; // Numero di UI aperte nella scena corrente
    #endregion

    [Header("References")]
    #region References
    public Tooltip tooltip; // Riferimento al tooltip per gli oggetti nell'inventario
    #endregion

    void Awake()
    {
        nUIOpen = 0; // Resettata a ogni nuova scena per evitare problemi (la precedente viene distrutta, quindi, anche se alcune saranno aperte prima del cambio, io la azzero all'inizio della nuova scena)
        ToggleCursor(SceneManager.GetActiveScene().name == "MainMenu");
        if (tooltip != null) InventoryUIController.instance.tooltip = tooltip;
    }

    public void GoToTransitionScene()
    {
        PlayerPrefs.SetString("CurrentChapter", nextChapter);
        PlayerPrefs.SetString("NextScene", nextScene);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Transizione");
    }

    public void SetItemScene(Item item)
    {
        if (item.nameItem.Contains("Zaino")) PlayerPrefs.SetInt("hasBackpack", 1);
        else if (item.nameItem.Contains("Torcia")) PlayerPrefs.SetInt("hasTorch", 1);
        else if (item.name.Contains("Bait")) PlayerPrefs.SetInt("hasBait", 1);

        PlayerPrefs.Save();
    }

    public static void ChangeCursorActiveStatus(bool isOpen, string debug)
    {
        // Gestione dell'apertura/chiusura delle UI
        nUIOpen += isOpen ? 1 : -1;
        nUIOpen = Mathf.Max(0, nUIOpen);  // Assicura che il valore non scenda mai sotto 0

        print($"UI aperte: ('prima' {(isOpen ? +1 : -1)}) {nUIOpen} (invocata in {debug})");
        ToggleCursor(nUIOpen > 0);  // Cambia visibilità del cursore se almeno una UI è aperta
    }

    public static void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
