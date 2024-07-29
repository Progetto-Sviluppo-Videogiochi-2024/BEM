using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Importa il namespace di TextMeshPro

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private Color glitchColor = Color.red; // Colore del glitch
    [SerializeField] private float glitchDuration = 0.1f; // Durata di ogni glitch
    [SerializeField] private float glitchInterval = 0.5f; // Intervallo tra i glitch
    [SerializeField] private float shakeIntensity = 5f; // Intensità del tremolio

    private AudioSource audioSource;
    private TextMeshProUGUI nameGameText; // Campo per il componente TextMeshProUGUI
    private Color originalColor;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        // Aggiungi un componente AudioSource se non esiste
        audioSource = gameObject.AddComponent<AudioSource>();

        // Imposta il clip audio
        audioSource.clip = backgroundMusic;

        // Configura l'AudioSource
        audioSource.loop = true; // Imposta loop su true per riprodurre l'audio in loop
        audioSource.playOnAwake = true;

        // Riproduci l'audio
        audioSource.Play();

        // Trova l'oggetto NameGame e il suo TextMeshProUGUI
        GameObject nameGameObject = GameObject.Find("MenuCanvas/StartUI/NameGame");
        if (nameGameObject != null)
        {
            nameGameText = nameGameObject.GetComponent<TextMeshProUGUI>();
            if (nameGameText != null)
            {
                originalColor = nameGameText.color;
                originalPosition = nameGameText.transform.position;
                originalRotation = nameGameText.transform.rotation;
                StartCoroutine(GlitchEffectCoroutine());
            }
            else
            {
                Debug.LogError("Componente TextMeshProUGUI non trovato su NameGame.");
            }
        }
        else
        {
            Debug.LogError("Oggetto NameGame non trovato nella gerarchia.");
        }
    }

    public void PlayGame()
    {
        // Ferma la musica quando si avvia il gioco
        audioSource.Stop();
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        // Ferma la musica e chiudi il gioco
        audioSource.Stop();
        Application.Quit();
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
                    nameGameText.transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
                    nameGameText.transform.rotation = Quaternion.Euler(
                        originalRotation.eulerAngles + new Vector3(
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity),
                            Random.Range(-shakeIntensity, shakeIntensity)
                        )
                    );
                    shakeTime -= Time.deltaTime;
                    yield return null;
                }

                // Ripristina la posizione e la rotazione originali
                nameGameText.transform.position = originalPosition;
                nameGameText.transform.rotation = originalRotation;

                // Ripristina il colore
                nameGameText.color = originalColor;
                yield return new WaitForSeconds(glitchInterval);
            }
            else
            {
                yield break; // Interrompe il ciclo se l'elemento TextMeshProUGUI non è stato assegnato
            }
        }
    }
}
