using UnityEngine;
using TMPro; // Necessario per gestire il componente TextMeshProUGUI

public class TriggerToolTip : MonoBehaviour
{
    public GameObject tooltip; // Assegna qui il prefab del tooltip tramite l'Inspector
    public string tooltipMessage = "Cambia questo testo"; // Messaggio da mostrare nel tooltip

    private void Start()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false); // Disabilita il tooltip all'inizio
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se l'oggetto che entra è il Player (tag "Player")
        if (other.CompareTag("Player"))
        {
            if (tooltip != null)
            {
                tooltip.SetActive(true); // Attiva il tooltip
                // Modifica il testo del tooltip (assicurati che il prefab contenga TextMeshProUGUI)
                tooltip.transform.GetComponentInChildren<TextMeshProUGUI>().text = tooltipMessage;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Quando il Player esce dal trigger, disattiva il tooltip
        if (other.CompareTag("Player"))
        {
            if (tooltip != null)
            {
                tooltip.SetActive(false);
            }
        }
    }
}
