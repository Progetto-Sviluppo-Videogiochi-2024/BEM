using System.Collections;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    private float launchForce = 10f; // Forza del lancio
    [SerializeField] Vector3 additionalDirection = Vector3.up; // Direzione aggiuntiva per il lancio
    #endregion

    [Header("References")]
    #region References
    public HumanFollower human; // Riferimento al personaggio umano
    private Animator animator; // Riferimento all'Animator
    public Animator goalkeeperAnimator; // Riferimento all'Animator del portiere
    #endregion

    void Start()
    {
        if (BooleanAccessor.istance.GetBoolFromThis("videoMutant")) { enabled = false; return; }
        animator = human.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il personaggio entra nel trigger
        if (other.CompareTag("Angelica"))
        {
            // Avvia l'animazione del calcio
            animator.SetTrigger("KickBall");
            goalkeeperAnimator.SetTrigger("KickBall");
            StartCoroutine(LaunchBallWithDelay(0.4f));
            StartCoroutine(DisableAnimatorWithDelay(1.5f));
        }
    }

    private IEnumerator LaunchBallWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!TryGetComponent(out Rigidbody rb))
        {
            Debug.LogWarning("Rigidbody not found on the Ball GameObject.");
            yield break;
        }

        Vector3 launchDirection = (transform.position - transform.position).normalized + additionalDirection; // Calcola la direzione del lancio
        rb.AddForce(launchDirection * launchForce, ForceMode.Impulse); // Applica una forza per lanciare la palla
        human.HitBall = true;
    }

    private IEnumerator DisableAnimatorWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        goalkeeperAnimator.SetBool("Portiere", false);
        enabled = false;
    }
}
