using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLookAt : MonoBehaviour
{
    public Transform player;
    public Transform cameraTransform;
    public float smoothSpeed = 0.125f;
    public float heightOffset = 1.5f; // Altezza desiderata rispetto al personaggio

    void LateUpdate()
    {
        // GameObject soldier = GameObject.Find("Player");

        // Calcola la posizione desiderata della telecamera dietro al personaggio
        Vector3 desiredPosition = player.position;
        desiredPosition -= player.forward * 5f; // Distanza dietro al personaggio
        desiredPosition.y += heightOffset; // Altezza rispetto al personaggio

        // Movimento fluido verso la posizione desiderata
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Guarda sempre il personaggio
        transform.LookAt(player);
    }
}
