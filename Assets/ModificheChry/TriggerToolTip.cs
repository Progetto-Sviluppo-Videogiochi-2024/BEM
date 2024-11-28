using UnityEngine;
using TMPro; // Necessario per gestire il componente TextMeshProUGUI
using System.Collections; // Per utilizzare le coroutine

public class TriggerToolTip : MonoBehaviour
{
    [Header("Tooltip Settings")]
    public GameObject tooltip; // Assegna qui il prefab del tooltip tramite l'Inspector
    public string tooltipMessage = "Cambia questo testo"; // Messaggio predefinito da mostrare nel tooltip
    public float tooltipDuration = 2f; // Durata del tooltip in secondi (opzionale)
    //private bool isPlayerInRange = false; // Stato: il player è nell'area del trigger
    private bool isTooltipActive = false; // Stato: il tooltip è attivo

    private void Start()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false); // Disabilita il tooltip all'inizio
            //Debug.Log("Tooltip inizializzato e disattivato.");
        }
        else
        {
            Debug.LogWarning("Tooltip non assegnato nell'Inspector. Verifica la configurazione.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //isPlayerInRange = true; // Aggiorna lo stato
            ShowTooltip(tooltipMessage); // Mostra il tooltip con il messaggio predefinito
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //isPlayerInRange = false; // Aggiorna lo stato
            HideTooltip(); // Nascondi il tooltip
        }
    }

    // Mostra il tooltip e imposta il messaggio.
    public void ShowTooltip(string message)
    {
        if (!isTooltipActive && tooltip != null)
        {
            tooltip.SetActive(true); // Attiva il tooltip
            SetTooltipMessage(message); // Imposta il messaggio
            isTooltipActive = true;

            // Se è specificata una durata, disabilita automaticamente il tooltip
            if (tooltipDuration > 0)
            {
                StartCoroutine(DisableTooltipAfterTime(tooltipDuration));
            }

        }
    }

    public void HideTooltip()
    {
        if (isTooltipActive && tooltip != null)
        {
            tooltip.SetActive(false); // Disattiva il tooltip
            isTooltipActive = false;
        }
    }

    public void SetTooltipMessage(string message)
    {
        if (tooltip != null)
        {
            var textComponent = tooltip.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = message;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI non trovato nel tooltip. Verifica la struttura del prefab.");
            }
        }
    }

    // Coroutine per disattivare automaticamente il tooltip dopo un certo tempo.
    private IEnumerator DisableTooltipAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTooltip(); // Nasconde il tooltip
    }
}
