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

    void Awake()
    {
        nUIOpen = 0; // Resettata a ogni nuova scena per evitare problemi (la precedente viene distrutta, quindi, anche se alcune saranno aperte prima del cambio, io la azzero all'inizio della nuova scena)
        ToggleCursor(SceneManager.GetActiveScene().name == "MainMenu");
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
    }

    public static void ChangeCursorActiveStatus(bool isOpen, string debug)
    {
        // Gestione dell'apertura/chiusura delle UI
        nUIOpen += isOpen ? 1 : -1;
        nUIOpen = Mathf.Max(0, nUIOpen);  // Assicura che il valore non scenda mai sotto 0

        print("UI aperte: " + nUIOpen + " (invocata in " + debug + ")");
        ToggleCursor(nUIOpen > 0);  // Cambia visibilità del cursore se almeno una UI è aperta
    }

    public static void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
