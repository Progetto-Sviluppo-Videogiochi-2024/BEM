using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNextScene : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private AudioSource audioSource; // Variabile per AudioSource
    [SerializeField] private AudioClip startAudioClip; // Clip audio da riprodurre all'inizio della scena
    [SerializeField] private float audioVolume = 0.01f; // Volume molto basso
    private bool isMuted = false; // Stato di mutamento dell'audio

    // Start is called before the first frame update
    void Start()
    {
        // Non aggiungere un nuovo AudioSource se non è assegnato, ma avvisa se non è stato assegnato
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non assegnato nello script GoNextScene. Assegna un AudioSource tramite l'Inspector.");
            return;
        }

        // Imposta il clip audio e altre proprietà solo se l'AudioSource è stato assegnato
        if (startAudioClip != null)
        {
            audioSource.clip = startAudioClip;
            audioSource.playOnAwake = false; // Imposta su false se non vuoi che l'audio parta automaticamente
            audioSource.loop = false; // Imposta su true se vuoi che il clip audio si ripeta
            audioSource.volume = audioVolume; // Imposta il volume molto basso
            audioSource.Play(); // Avvia la riproduzione dell'audio
        }
        else
        {
            Debug.LogWarning("AudioClip non assegnato in GoNextScene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(sceneName);
        }

        // Controlla se il tasto 'M' o 'm' viene premuto
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    // Metodo per mutare o riattivare l'audio
    private void ToggleMute()
    {
        isMuted = !isMuted; // Inverte lo stato di mutamento
        if (audioSource != null)
        {
            audioSource.mute = isMuted; // Imposta la muta per l'AudioSource
            Debug.Log(isMuted ? "Audio Mutato" : "Audio Ripristinato");
        }
    }
}
