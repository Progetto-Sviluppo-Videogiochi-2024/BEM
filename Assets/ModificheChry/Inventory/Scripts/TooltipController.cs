using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject tooltip; // Tooltip dell'inventario
    private TextMeshProUGUI tooltipText; // Testo del tooltip

    private void Start()
    {
        tooltip = transform.gameObject;
        tooltipText = tooltip.transform.Find("TextTooltip").GetComponent<TextMeshProUGUI>();
        tooltipText.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var weightSlider = InventoryUIController.instance.weightSlider;
        tooltipText.gameObject.SetActive(true);
        tooltipText.text = $"Peso: {weightSlider.value} / {weightSlider.maxValue} kg";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipText.gameObject.SetActive(false);
    }
}
