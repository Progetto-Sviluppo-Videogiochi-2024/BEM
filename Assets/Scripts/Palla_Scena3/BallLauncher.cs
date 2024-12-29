using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    public GameObject ball; // Riferimento alla palla
    public float launchForce = 10f; // Forza del lancio
    public Vector3 additionalDirection = Vector3.up; // Direzione aggiuntiva per il lancio
    public HumanFollower human;
    private void OnTriggerEnter(Collider other)
    {   
        // Controlla se il personaggio entra nel trigger
        if (other.CompareTag("Player"))
        {
            // Recupera il Rigidbody della palla
            if (ball != null)
            {
                Rigidbody rb = ball.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calcola la direzione del lancio (dalla posizione del trigger verso la palla)
                    Vector3 launchDirection = (ball.transform.position - transform.position).normalized + additionalDirection;

                    // Applica una forza per lanciare la palla
                    rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
                    
                    human.HitBall = true;
                }
                else
                {
                    Debug.LogWarning("Rigidbody non trovato sulla palla.");
                }
            }
            else
            {
                Debug.LogWarning("Oggetto palla non assegnato.");
            }
        }
    }
}
