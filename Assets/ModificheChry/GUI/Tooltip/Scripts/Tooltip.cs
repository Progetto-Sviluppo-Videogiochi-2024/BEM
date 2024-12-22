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
        if (gameObject.activeSelf && head != null)
        {
            Vector3 worldPosition = head.position + head.TransformDirection(offset);
            transform.position = mainCamera.WorldToScreenPoint(worldPosition);
            DebugLogger.Log($"Tooltip.update: {transform.position} - {worldPosition} - {head.position}");
        }
    }

    public void ShowTooltip(string message, float tooltipDuration)
    {
        DebugLogger.Log($"Tooltip.show pre if: {isTooltipActive} == {gameObject.activeSelf}");
        if (!isTooltipActive)
        {
            gameObject.SetActive(true);
            SetTooltipMessage(message);
            isTooltipActive = true;
            DebugLogger.Log($"Tooltip.show: '{message}' - Duration: {tooltipDuration} seconds. Active: {isTooltipActive} == {gameObject.activeSelf}");
        }
        if (tooltipDuration > 0)
        {
            StopAllCoroutines(); // Ferma eventuali coroutine precedenti per evitare conflitti
            StartCoroutine(DisableTooltipAfterTime(tooltipDuration));
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
        DebugLogger.Log($"Tooltip.disable coroutine started: Waiting {delay} seconds.");

        float elapsedTime = 0f;

        while (elapsedTime < delay)
        {
            if (!gameObject.activeSelf) // Se viene disattivato manualmente, interrompi
            {
                DebugLogger.Log("Tooltip.disable coroutine: GameObject was deactivated externally.");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            DebugLogger.Log($"Tooltip.disable coroutine: {elapsedTime} / {delay} seconds.");
            yield return null;
        }

        HideTooltip();
        DebugLogger.Log($"Tooltip.disable: Tooltip hidden after {delay} seconds.");
    }
}
