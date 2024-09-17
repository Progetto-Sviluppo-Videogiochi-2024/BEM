using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionWithAudio : MonoBehaviour
{
    [Header("Audio Properties")]
    [SerializeField] private AudioSource audioSource; // Variabile per AudioSource
    [SerializeField] private AudioClip audioClip; // Clip audio da riprodurre all'inizio della scena
    [SerializeField] private float audioVolume; // Volume dell'audio
    private bool isMuted = false; // Stato di mutamento dell'audio

    void Start()
    {
        PlayAudio();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) // Se 'Tab'
        {
            SceneManager.LoadScene(GetComponent<ConversationCharacters>().GetNextScene()); // Per caricare la scena successiva
        }

        if (Input.GetKeyDown(KeyCode.M)) // Se 'M'/'m'
        {
            ToggleMute();
        }
    }

    // Metodo per avviare l'audio
    private void PlayAudio()
    {
        if (audioSource == null) // Se l'AudioSource non è assegnato
        {
            Debug.LogWarning("AudioSource don't assigned to the script. Please assign it in the inspector.");
            return;
        }

        if (audioClip != null) // Se la clip audio è stata assegnata
        {
            audioSource.clip = audioClip; // Per impostare la clip audio
            audioSource.playOnAwake = false; // Per evitare la riproduzione automatica
            audioSource.loop = false; // Per evitare la ripetizione
            audioSource.volume = audioVolume; // Per impostare il volume
            audioSource.Play(); // Per avviarlo
        }
        else // Se la clip audio non è stata assegnata
        {
            Debug.LogWarning("AudioClip don't assigned to the script. Please assign it in the inspector.");
        }
    }

    // Metodo per mutare o riattivare l'audio
    private void ToggleMute()
    {
        isMuted = !isMuted;
        if (audioSource != null) // Se l'AudioSource è assegnato
        {
            audioSource.mute = isMuted;
        }
    }
}
