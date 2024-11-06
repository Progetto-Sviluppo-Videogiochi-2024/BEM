using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    private GestoreScena transition;

    [Header("UI Glitch Settings")]
    [SerializeField] private Color glitchColor = Color.red; // Colore del glitch
    [SerializeField] private float glitchDuration = 0.1f; // Durata di ogni glitch
    [SerializeField] private float glitchInterval = 0.5f; // Intervallo tra i glitch
    [SerializeField] private float shakeIntensity = 5f; // Intensità del tremolio

    [Header("UI Elements")]
    [SerializeField] GameObject nameGameObject; // Riferimento all'oggetto NameGame
    private TextMeshProUGUI nameGameText; // Campo per il componente TextMeshProUGUI
    private Color originalColor; // Colore originale del testo
    private Vector3 originalPosition; // Posizione originale del testo
    private Quaternion originalRotation; // Rotazione originale del testo

    [Header("Audio Settings")]
    public AudioClip audioClip; // Clip audio per la musica di sottofondo
    private AudioSource audioSource; // AudioSource per la musica di sottofondo

    void Start()
    {
        transition = FindObjectOfType<GestoreScena>();

        // Avvia la musica di sottofondo
        PlayAudio();

        // Crea l'effetto glitch sul testo
        CreateGlitchText();
    }

    void Update()
    {
        ActivateMenuFromKeyboard();
    }

    private void ActivateMenuFromKeyboard()
    {
        // TODO: implementare le feature del menu con la tastiera
        // problema: troppi riferimenti se come attributi del cs
        // oppure usare metodi che prendono come stringa i path dei GameObject
    }

    public void StartGame()
    {
        // Ferma la musica quando si avvia il gioco
        StopAudio();

        transition.SetNextScene();
    }

    public void QuitGame()
    {
        // Ferma la musica e chiudi il gioco
        StopAudio();

        Application.Quit();
    }

    private void PlayAudio()
    {
        // Aggiungi un componente AudioSource se non esiste
        audioSource = GetComponent<AudioSource>();

        // Imposta il clip audio
        audioSource.clip = audioClip;

        // Configura l'AudioSource
        audioSource.loop = true; // Imposta loop su true per riprodurre l'audio in loop
        audioSource.playOnAwake = true;

        // Riproduci l'audio
        audioSource.Play();
    }

    private void StopAudio()
    {
        // Interrompi l'audio
        audioSource.Stop();
    }

    private void CreateGlitchText()
    {
        // GameObject nameGameObject = GameObject.Find("Menu/StartUI/NameGame"); // Trova l'oggetto NameGame e il suo TextMeshProUGUI
        if (nameGameObject != null)
        {
            if (nameGameObject.TryGetComponent<TextMeshProUGUI>(out nameGameText))
            {
                originalColor = nameGameText.color;
                originalPosition = nameGameText.transform.position;
                originalRotation = nameGameText.transform.rotation;
                StartCoroutine(GlitchEffectCoroutine());
            }
            else Debug.LogError("Componente TextMeshProUGUI non trovato su NameGame.");
        }
        else Debug.LogError("Oggetto NameGame non trovato nella gerarchia.");
    }

    private IEnumerator GlitchEffectCoroutine()
    {
        while (true)
        {
            // Cambia il colore e applica tremolio
            if (nameGameText != null)
            {
                nameGameText.color = glitchColor;

                // Applica tremolio alla posizione e alla rotazione
                float shakeTime = glitchDuration;
                while (shakeTime > 0)
                {
                    nameGameText.transform.SetPositionAndRotation(originalPosition + Random.insideUnitSphere * shakeIntensity, Quaternion.Euler(
                        originalRotation.eulerAngles + new Vector3(
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity)
                        )
                    ));
                    shakeTime -= Time.deltaTime;
                    yield return null;
                }

                // Ripristina la posizione e la rotazione originali
                nameGameText.transform.SetPositionAndRotation(originalPosition, originalRotation);

                // Ripristina il colore
                nameGameText.color = originalColor;
                yield return new WaitForSeconds(glitchInterval);
            }
            else yield break; // Interrompe il ciclo se l'elemento TextMeshProUGUI non è stato assegnato
        }
    }
}
