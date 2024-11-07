using System.Collections;
using UnityEngine;

public class ThunderSound : MonoBehaviour
{
    private AudioSource audioSource;  // Riferimento all'AudioSource del tuono
    public float fadeDuration = 3f;   // Durata del fade-out (in secondi)

    private void Start()
    {
        // Ottieni il riferimento all'AudioSource
        audioSource = GetComponent<AudioSource>();

        // Inizia a riprodurre il suono
        audioSource.Play();

        // Avvia il fade-out dopo un certo tempo (ad esempio 2 secondi)
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // Inizializza la durata del fade
        float startVolume = audioSource.volume;

        // Esegui il fade-out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // Riduci il volume in modo graduale
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        // Alla fine del fade, imposta il volume a 0 e ferma il suono
        audioSource.volume = 0;
        audioSource.Stop();  // Arresta il suono dopo il fade-out
    }
}
