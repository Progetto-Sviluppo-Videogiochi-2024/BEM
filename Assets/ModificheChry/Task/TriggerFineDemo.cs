using UnityEngine;
using UnityEngine.AI;

public class TriggerFineDemo : MonoBehaviour
{
    public Transform player; // Riferimento al player
    public NPCAIWithWaypoints gaia; // Riferimento all'AI di Gaia
    public NPCAIWithWaypoints jacob; // Riferimento all'AI di Jacob
    public NPCAIWithWaypoints angelica; // Riferimento all'AI di Angelica

    bool isTriggered = false; // Flag per controllare se il trigger è stato attivato

    void OnTriggerEnter(Collider other)
    {
        // Per il player
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;

            var animator = player.GetComponent<Animator>();
            player.GetComponent<MovementStateManager>().enabled = false;
            animator.SetBool("walking", false);
            animator.SetBool("running", false);
            animator.SetBool("crouching", false);
            animator.SetFloat("hInput", 0);
            animator.SetFloat("vInput", 0);

            // TODO: Fare altro per concludere la demo @marcoWarrior @NicL28
        }

        // Per le AI NPC
        if (other.gameObject.GetComponentInParent<NavMeshAgent>() != null)
        {
            other.gameObject.GetComponent<NPCAIWithWaypoints>().DisableAI();
        }
    }
}
