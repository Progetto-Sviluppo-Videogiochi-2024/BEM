using UnityEngine;
using UnityEngine.SceneManagement;

public class GestoreScena : MonoBehaviour
{
    public string chapterName = "Chapter i:";
    public string nextScene = "ScenaI";

    public void SetNextScene()
    {
        PlayerPrefs.SetString("CurrentChapter", chapterName);
        PlayerPrefs.SetString("NextScene", nextScene);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Transizione");
    }
}