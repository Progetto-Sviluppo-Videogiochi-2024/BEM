using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreScena : MonoBehaviour
{
    [Header("Next Scene")]
    #region Next Scene
    public string nextChapter = "Chapter I:";
    public string nextScene = "ScenaI";
    #endregion

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
}
