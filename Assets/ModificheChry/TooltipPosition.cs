using UnityEngine;

public class TooltipPosition : MonoBehaviour
{
    public Transform head; // Riferimento alla testa del personaggio
    public Vector3 offset = new(1.5f, 0, 0); // Offset verso destra
    private Camera mainCamera; // Riferimento alla camera principale

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gameObject.activeSelf) transform.position = mainCamera.WorldToScreenPoint(head.position + offset);
    }
}
