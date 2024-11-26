using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreScena : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private bool cursor = false;
    #endregion

    [Header("Next Scene")]
    #region Next Scene
    public string nextChapter = "Chapter I:";
    public string nextScene = "ScenaI";
    #endregion

    void Awake()
    {
        // Nasconde il cursore e lo blocca al centro
        if (SceneManager.GetActiveScene().name == "MainMenu") ToggleCursor(true);
        else ToggleCursor(false);
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

    private void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
