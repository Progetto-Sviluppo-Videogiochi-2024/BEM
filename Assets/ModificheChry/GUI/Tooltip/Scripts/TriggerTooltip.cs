using UnityEngine;

// Da usare per esprimere riflessioni di Stefano su ciò che gli circonda
public class TriggerTooltip : MonoBehaviour
{
    [Header("References")]
    #region References
    public Tooltip tooltip;
    #endregion

    [Header("Settings")]
    #region Settings
    public string tooltipMessage = "Cambia questo testo";
    public float tooltipDuration = 2f;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tooltip.ShowTooltip(tooltipMessage, tooltipDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tooltip.HideTooltip();
        }
    }
}
