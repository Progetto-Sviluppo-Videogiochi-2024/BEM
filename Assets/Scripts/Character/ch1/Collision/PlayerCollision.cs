using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Se il giocatore entra nella zona di collisione con un ostacolo
        if (other.CompareTag("Obstacle"))
        {
            // Calcola la direzione dalla posizione del giocatore all'ostacolo
            Vector3 direction = other.transform.position - transform.position;

            // Imposta la componente Y e Z a zero per evitare movimenti verticali e laterali
            direction.y = 0f;
            direction.z = 0f;

            // Normalizza la direzione per mantenere solo la direzione
            direction.Normalize();

            // Ottieni la posizione corrente del giocatore
            Vector3 currentPosition = transform.position;

            // Calcola la nuova posizione corretta per il giocatore, appena di fronte all'ostacolo
            Vector3 newPosition = currentPosition - direction * 1.5f; // Regola 1.5f in base alla distanza che desideri mantenere tra il giocatore e l'ostacolo

            // Imposta la nuova posizione del giocatore
            transform.position = newPosition;
        }
    }
}

