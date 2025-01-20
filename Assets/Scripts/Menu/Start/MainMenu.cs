using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    public GestoreScena gestoreScena; // Riferimento al GestoreScena per la transizione tra le scene

    [Header("UI Elements")]
    [SerializeField] GameObject title; // GameObject per il titolo del gioco
    public Transform slotUI; // Indica la lista degli slot di salvataggio (da configurare a runtime sulla base dei suoi slot creati e salvati)

    [Header("Audio Settings")]
    public AudioClip audioClip; // Clip audio per la musica di sottofondo
    private AudioSource audioSource; // AudioSource per la musica di sottofondo

    void Start()
    {
        PlayAudio(); // Avvia la musica di sottofondo
        title.GetComponent<GlitchEffect>().CreateGlitch(); // Avvia l'effetto glitch sul titolo del gioco

        // InventoryManager.instance.transform.parent?.gameObject.SetActive(false); // Nasconde l'inventario all'avvio
        Destroy(InventoryManager.instance?.transform.parent?.gameObject); // Distrugge l'inventario all'avvio (se presente)
    }

    public void StartGame()
    {
        StopAudio(); // Ferma la musica quando si avvia il gioco
        gestoreScena.GoToTransitionScene(); // Avvia la scena transizione
    }

    // public void LoadGame() { } // Implementato già nell'inspector del button Continue del MM

    public void QuitGame()
    {
        StopAudio(); // Ferma la musica
        Application.Quit(); // Chiudi l'applicazione
    }

    private void PlayAudio()
    {
        // Aggiungi un componente AudioSource se non esiste
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.loop = true; // Imposta loop su true per riprodurre l'audio in loop
        audioSource.playOnAwake = true;
        audioSource.Play();
    }

    private void StopAudio() => audioSource.Stop();
}
