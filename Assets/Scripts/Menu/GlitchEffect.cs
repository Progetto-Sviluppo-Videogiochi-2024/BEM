using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlitchEffect : MonoBehaviour
{
    [Header("UI Glitch Settings")]
    [SerializeField] private Color glitchColor = Color.red; // Colore del glitch
    [SerializeField] private float glitchDuration = 0.1f; // Durata di ogni glitch
    [SerializeField] private float glitchInterval = 0.5f; // Intervallo tra i glitch
    [SerializeField] private float shakeIntensity = 5f; // Intensità del tremolio
    TextMeshProUGUI textComponent; // Componente TextMeshProUGUI

    private bool isButton = false; // Variabile per controllare se il GO è un Button
    private bool isGlitching = false; // Variabile per controllare lo stato del glitch

    void Awake()
    {
        if (TryGetComponent<Button>(out var button)) // Se il GO è un Button
        {
            textComponent = button.GetComponentInChildren<TextMeshProUGUI>();
            isButton = true;
        }
        else // Se il GO è un TextMeshProUGUI
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }

        // Controllo di validità
        if (textComponent == null)
        {
            Debug.LogError("TextMeshProUGUI non trovato su " + name);
            return;
        }
    }

    public void CreateGlitch()
    {
        if (!isGlitching && textComponent != null) // Avvia il glitch solo se non è già attivo
        {
            isGlitching = true; // Segna il glitch come attivo
            Color originalColor = textComponent.color;
            textComponent.transform.GetPositionAndRotation(out Vector3 originalPosition, out Quaternion originalRotation);
            StartCoroutine(GlitchEffectCoroutine(textComponent, originalColor, originalPosition, originalRotation));
        }
    }

    IEnumerator GlitchEffectCoroutine(TextMeshProUGUI textUI, Color originalColor, Vector3 originalPosition, Quaternion originalRotation)
    {
        while (true)
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

            // Ripristina colore, posizione e rotazione originali
            textUI.transform.SetPositionAndRotation(originalPosition, originalRotation);
            textUI.color = originalColor;

            isGlitching = false;
            if (!isButton) yield return new WaitForSeconds(glitchInterval); // Se il GO non è un Button, attende l'intervallo tra i glitch
            else yield break; // Se il GO è un Button, esce dal ciclo
        }
    }
}
