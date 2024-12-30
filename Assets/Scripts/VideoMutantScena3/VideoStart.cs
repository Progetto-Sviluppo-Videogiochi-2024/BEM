using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoStart : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Riferimento al Video Player

    private void Start()
    {
        if (videoPlayer != null)
        {
            // TODO: SFX per il video @marcoWarrior
            videoPlayer.loopPointReached += VideoFinished; // Registra l'evento
            videoPlayer.Play(); // Riproduci il video
        }
    }

    private void VideoFinished(VideoPlayer vp)
    {
        PlayerPrefs.SetInt("videoMutant", 1);
        PlayerPrefs.Save();
        BooleanAccessor.istance.SetBoolOnDialogueE("videoMutant");

        SceneManager.LoadScene("Scena3");
    }
}
