using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoTransitionManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Riferimento al Video Player per il video del mutante che attacca i militari
    private string sceneToLoad = "Scena3Video"; // Nome della scena contenente il video

    // private void Start() => Invoke("StartVideo", 5f); // Per testing

    public void StartVideo() => LoadVideoScene(); 

    private void LoadVideoScene() => SceneManager.LoadScene(sceneToLoad);

    // public void PlayVideoAndReturn()
    // {
    //     if (videoPlayer != null)
    //     {
    //         videoPlayer.loopPointReached += LoadPreviousScene; // Registra l'evento
    //         videoPlayer.Play(); // Riproduci il video
    //     }
    // }

    // private void LoadPreviousScene(VideoPlayer vp) => SceneManager.LoadScene("Scena3"); // Torna alla scena originale
}
