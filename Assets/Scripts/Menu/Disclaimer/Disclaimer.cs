using UnityEngine;
using UnityEngine.SceneManagement;

public class Disclaimer : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("MainMenu");
    public void QuitGame() => Application.Quit();
}
