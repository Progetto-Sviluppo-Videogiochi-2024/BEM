using System.Collections;
using UnityEngine;
using TMPro;

public class ChapterTransition : MonoBehaviour
{
    public TextMeshProUGUI chapterText;    // Riferimento al testo del capitolo
    public TextMeshProUGUI continueText;   // Riferimento al testo "Continua"
    public float displayDuration = 4f;     // Tempo di visualizzazione del testo capitolo
    public float fadeDuration = 2f;        // Durata del fade in
    public float glitchDuration = 0.1f;    // Durata di ogni effetto glitch
    public float glitchInterval = 0.5f;    // Intervallo tra glitch
    public float shakeIntensity = 5f;      // Intensità del tremolio

    private Color originalColor;
    private Vector3 originalPosition;

    private void Start()
    {
        chapterText.alpha = 0;       // Inizia con testo capitolo invisibile
        continueText.alpha = 0;      // Inizia con "Continua" invisibile
        StartCoroutine(ShowChapterText());
    }

    // Funzione pubblica per impostare il testo del capitolo in runtime
    public void SetChapterText(string chapterTitle)
    {
        if (chapterText != null)
        {
            chapterText.text = chapterTitle;
        }
    }

    private IEnumerator ShowChapterText()
    {
        // Fade in per il testo del capitolo
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            chapterText.alpha = t / fadeDuration;
            yield return null;
        }
        chapterText.alpha = 1;  // Assicurati che sia visibile al 100%

        // Attendi per il displayDuration impostato
        yield return new WaitForSeconds(displayDuration);

        // Fade in per "Continua"
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            continueText.alpha = t / fadeDuration;
            yield return null;
        }
        continueText.alpha = 1;

        // Inizia l’effetto glitch su "Continua"
        originalColor = continueText.color;
        originalPosition = continueText.transform.position;
        StartCoroutine(GlitchEffect());
    }

    private IEnumerator GlitchEffect()
    {
        while (true)
        {
            // Cambia il colore per l'effetto glitch temporaneo
            continueText.color = Color.red;

            float shakeTime = glitchDuration;
            while (shakeTime > 0)
            {
                // Effetto di tremolio su posizione
                continueText.transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
                shakeTime -= Time.deltaTime;
                yield return null;
            }

            // Reset posizione e colore
            continueText.transform.position = originalPosition;
            continueText.color = originalColor;

            yield return new WaitForSeconds(glitchInterval);
        }
    }
}
