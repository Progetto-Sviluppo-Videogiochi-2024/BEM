using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TriggerFineDemo : MonoBehaviour
{
    public Transform player; // Riferimento al player
    public NPCAIWithWaypoints gaia; // Riferimento all'AI di Gaia
    public NPCAIWithWaypoints jacob; // Riferimento all'AI di Jacob
    public NPCAIWithWaypoints angelica; // Riferimento all'AI di Angelica
    public Diario diario; // Riferimento al diario
    public GestoreScena gestore;
    bool isTriggered = false; // Flag per controllare se il trigger è stato attivato

    IEnumerator EndDemo()
    {
        yield return new WaitForSeconds(2f);
        gestore.GoToTransitionScene();
    }

    void OnTriggerEnter(Collider other)
    {
        // Per il player
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            diario.CompletaMissione("Fuggi nel tunnel");
            BooleanAccessor.istance.SetBoolOnDialogueE("endDemo");

            var animator = player.GetComponent<Animator>();
            player.GetComponent<MovementStateManager>().enabled = false;
            animator.SetBool("walking", false);
            animator.SetBool("running", false);
            animator.SetBool("crouching", false);
            animator.SetFloat("hInput", 0);
            animator.SetFloat("vInput", 0);

            if (BooleanAccessor.istance.GetBoolFromThis("endDemo") && BooleanAccessor.istance.GetBoolFromThis("postFrana"))
                StartCoroutine(EndDemo());
        }

        // Per le AI NPC
        if (other.gameObject.GetComponentInParent<NavMeshAgent>() != null)
        {
            other.gameObject.GetComponent<NPCAIWithWaypoints>().DisableAI();
        }
    }
}
