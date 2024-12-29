using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    public GameObject ball; // Riferimento alla palla
    public float launchForce = 10f; // Forza del lancio
    public Vector3 additionalDirection = Vector3.up; // Direzione aggiuntiva per il lancio
    public HumanFollower human;
    public Animator animator; // Riferimento all'Animator
    public Animator goalkeeperAnimator; // Riferimento all'Animator del portiere

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il personaggio entra nel trigger
        if (other.CompareTag("Angelica"))
        {
            // Avvia l'animazione del calcio
            if (animator != null)
            {
                animator.SetTrigger("KickTrigger"); // Imposta il trigger per l'animazione
            }
            else
            {
                Debug.LogWarning("Animator non assegnato.");
            }

            // Avvia l'animazione del portiere
            if (goalkeeperAnimator != null)
            {
                goalkeeperAnimator.SetTrigger("KickTrigger"); // Imposta lo stesso trigger anche per il portiere
            }
            else
            {
                Debug.LogWarning("Animator del portiere non assegnato.");
            }

            // Avvia il ritardo per lanciare la palla
            if (ball != null)
            {
                StartCoroutine(LaunchBallWithDelay(0.5f)); // Ritarda di 1 secondo
            }
            else
            {
                Debug.LogWarning("Oggetto palla non assegnato.");
            }
        }
    }

    private IEnumerator LaunchBallWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Recupera il Rigidbody della palla
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
}
