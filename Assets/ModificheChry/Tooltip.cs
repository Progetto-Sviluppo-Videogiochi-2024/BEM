using System.Collections;
using TMPro;
using UnityEngine;

// Da usare con: trigger e item raccoglibili
public class Tooltip : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    public Vector3 offset = new(1.5f, 0, 0); // Offset verso destra
    private bool isTooltipActive = false; // Stato del Tooltip
    #endregion

    [Header("References")]
    #region References
    private Camera mainCamera; // Riferimento alla camera principale
    public Transform head; // Riferimento alla testa del personaggio
    #endregion

    private void Start()
    {
        gameObject.SetActive(false);
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gameObject.activeSelf) transform.position = mainCamera.WorldToScreenPoint(head.position + offset);
    }

    public void ShowTooltip(string message, float tooltipDuration)
    {
        if (!isTooltipActive)
        {
            gameObject.SetActive(true);
            SetTooltipMessage(message);
            isTooltipActive = true;
            if (tooltipDuration > 0) StartCoroutine(DisableTooltipAfterTime(tooltipDuration));
        }
    }

    public void HideTooltip()
    {
        if (isTooltipActive)
        {
            gameObject.SetActive(false);
            isTooltipActive = false;
        }
    }

    public void SetTooltipMessage(string message) => gameObject.GetComponentInChildren<TextMeshProUGUI>().text = message;

    private IEnumerator DisableTooltipAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideTooltip();
    }
}
