using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class InfiniteForest : MonoBehaviour
{
    public GameObject[] roadSegments; // Array dei segmenti di strada
    public float segmentLength = 10.0f; // Lunghezza di ogni segmento
    public Transform vehicle; // Riferimento al veicolo
    public float SpaceNewPosition = 200;
    void Update()
    {
        foreach (GameObject segment in roadSegments)
        {
            // Controlla se il veicolo ha superato il segmento
            if (vehicle.position.z > segment.transform.position.z + segmentLength)
            {
                // Calcola la nuova posizione del segmento
                Vector3 newPos = segment.transform.position;
                newPos.z += (SpaceNewPosition * 3)-5; // Sposta il segmento in avanti
                segment.transform.position = newPos; // Applica la nuova posizione
            }
        }
    }
}
