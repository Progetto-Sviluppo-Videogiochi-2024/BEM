using System.Collections;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    private GestoreScena transition; // Riferimento al GestoreScena per la transizione tra le scene

    [Header("UI Glitch Settings")]
    [SerializeField] private Color glitchColor = Color.red; // Colore del glitch
    [SerializeField] private float glitchDuration = 0.1f; // Durata di ogni glitch
    [SerializeField] private float glitchInterval = 0.5f; // Intervallo tra i glitch
    [SerializeField] private float shakeIntensity = 5f; // Intensità del tremolio

    [Header("UI Elements")]
    [SerializeField] GameObject nameGameObject; // GameObject per il titolo del gioco
    private Coroutine glitchCoroutineTitle; // Per memorizzare la Coroutine attiva (per il titolo)
    private Coroutine glitchCoroutineButton; // Per memorizzare la Coroutine attiva (per i pulsanti)
    public Transform slotUI; // Indica la lista degli slot di salvataggio (da configurare a runtime sulla base dei suoi slot creati e salvati)

    [Header("Audio Settings")]
    public AudioClip audioClip; // Clip audio per la musica di sottofondo
    private AudioSource audioSource; // AudioSource per la musica di sottofondo

    void Start()
    {
        transition = FindObjectOfType<GestoreScena>();

        PlayAudio(); // Avvia la musica di sottofondo
        CreateGlitchText(); // Crea l'effetto glitch sul testo
        // InventoryManager.instance.transform.parent?.gameObject.SetActive(false); // Nasconde l'inventario all'avvio
        Destroy(InventoryManager.instance?.transform.parent?.gameObject); // Distrugge l'inventario all'avvio (se presente)
    }

    private void ActivateMenuFromKeyboard() // Invocarlo, se lo implementiamo, in Update
    {
        // TODO: implementare le feature del menu con la tastiera
        // problema: troppi riferimenti se come attributi del cs
        // oppure usare metodi che prendono come stringa i path dei GameObject
    }

    public void StartGame()
    {
        StopAudio(); // Ferma la musica quando si avvia il gioco
        transition.GoToTransitionScene();
    }

    // public void LoadGame() { } // Implementato già nell'inspector del button Continue del MM

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
        audioSource.clip = audioClip;
        audioSource.loop = true; // Imposta loop su true per riprodurre l'audio in loop
        audioSource.playOnAwake = true;
        audioSource.Play();
    }

    private void StopAudio() => audioSource.Stop();

    private void CreateGlitchText()
    {
        // GameObject nameGameObject = GameObject.Find("Menu/StartUI/NameGame"); // Trova l'oggetto NameGame e il suo TextMeshProUGUI
        if (nameGameObject != null)
        {
            if (nameGameObject.TryGetComponent(out TextMeshProUGUI nameGameText))
            {
                Color originalColor = nameGameText.color;
                Vector3 originalPosition = nameGameText.transform.position;
                Quaternion originalRotation = nameGameText.transform.rotation;
                StartCoroutine(GlitchEffectCoroutine(nameGameText, originalColor, originalPosition, originalRotation));
            }
            else Debug.LogError("Componente TextMeshProUGUI non trovato su NameGame.");
        }
        else Debug.LogError("Oggetto NameGame non trovato nella gerarchia.");
    }

    public void CreateGlitchText(GameObject buttonUI)
    {
        if (buttonUI != null)
        {
            var ButtonUIText = buttonUI.GetComponentInChildren<TextMeshProUGUI>();
            if (ButtonUIText != null)
            {
                bool oneTime = true; // Se true, l'effetto glitch si verifica una sola volta
                Color originalColor = ButtonUIText.color;
                Vector3 originalPosition = ButtonUIText.transform.position;
                Quaternion originalRotation = ButtonUIText.transform.rotation;

                // Avvia la Coroutine e salvala
                glitchCoroutineButton = StartCoroutine(GlitchEffectCoroutine(ButtonUIText, originalColor, originalPosition, originalRotation, oneTime));
            }
            else Debug.LogError("Componente TextMeshProUGUI non trovato su " + buttonUI.name);
        }
        else Debug.LogError("Oggetto buttonUI non trovato nella gerarchia.");
    }

    private IEnumerator GlitchEffectCoroutine(TextMeshProUGUI textUI, Color originalColor, Vector3 originalPosition, Quaternion originalRotation)
    {
        while (true)
        {
            if (textUI != null)
            {
                textUI.color = glitchColor;

                // Applica tremolio alla posizione e alla rotazione
                float shakeTime = glitchDuration;
                while (shakeTime > 0)
                {
                    textUI.transform.SetPositionAndRotation(
                        originalPosition + Random.insideUnitSphere * shakeIntensity,
                        Quaternion.Euler(originalRotation.eulerAngles + new Vector3(
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity)
                        ))
                    );
                    shakeTime -= Time.deltaTime;
                    yield return null;
                }

                // Ripristina la posizione e la rotazione originali
                textUI.transform.SetPositionAndRotation(originalPosition, originalRotation);

                // Ripristina il colore
                textUI.color = originalColor;
                yield return new WaitForSeconds(glitchInterval);
            }
            else yield break; // Interrompe il ciclo se l'elemento TextMeshProUGUI non è stato assegnato
        }
    }

    private IEnumerator GlitchEffectCoroutine(TextMeshProUGUI textUI, Color originalColor, Vector3 originalPosition, Quaternion originalRotation, bool oneTime)
    {
        if (textUI != null)
        {
            // Applica il colore del glitch
            textUI.color = glitchColor;

            // Applica tremolio alla posizione e alla rotazione
            float shakeTime = glitchDuration;
            while (shakeTime > 0)
            {
                textUI.transform.SetPositionAndRotation(
                    originalPosition + Random.insideUnitSphere * shakeIntensity,
                    Quaternion.Euler(originalRotation.eulerAngles + new Vector3(
                        Random.Range(-shakeIntensity, shakeIntensity),
                        Random.Range(-shakeIntensity, shakeIntensity),
                        Random.Range(-shakeIntensity, shakeIntensity)
                    ))
                );
                shakeTime -= Time.deltaTime;
                yield return null;
            }

            // Ripristina la posizione e la rotazione originali
            textUI.transform.SetPositionAndRotation(originalPosition, originalRotation);

            // Ripristina il colore originale
            textUI.color = originalColor;

            if (!oneTime)
            {
                // Se non è oneTime (quindi deve ripetersi in loop), ripeti l'effetto
                yield return new WaitForSeconds(glitchInterval);
                StartCoroutine(GlitchEffectCoroutine(textUI, originalColor, originalPosition, originalRotation, oneTime));
            }
        }
        else yield break; // Se textUI è null, esci dalla coroutine
    }
}
