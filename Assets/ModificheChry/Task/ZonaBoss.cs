using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DialogueEditor;

public class ZonaBoss : MonoBehaviour
{
    [Header("References")]
    #region References
    public Transform characters; // Riferimento ai personaggi della zona boss
    public Transform gaia; // Riferimento a Gaia
    private Transform angelica; // Riferimento ad Angelica
    private Transform jacob; // Riferimento a Jacob
    public Transform player; // Riferimento al player
    private Transform boss; // Riferimento al boss
    public CombinazioneManager combinazioneManager; // Riferimento al CombinazioneManager
    public Transform[] newWaypoints; // Nuovi waypoints per Jacob, Angelica e Gaia
    public Transform hideoutGaia; // Nascondiglio di Gaia quando Stefano combatte con il boss
    #endregion

    [Header("Conversations")]
    #region Conversations
    public NPCConversation[] conversations;
    private ConversationManager conversationManager;
    private bool JacobAngelicaLiberi = false; // Flag per riprodurre la conversazione una sola volta
    #endregion

    // Le missioni le stiamo gestendo dai due dialoghi Riunione&Boss e BossStunned  

    void Start()
    {
        angelica = characters.GetChild(0);
        jacob = characters.GetChild(1);
        boss = characters.GetChild(2);

        var doorUnlock = BooleanAccessor.istance.GetBoolFromThis("doorUnlocked");
        angelica.gameObject.SetActive(doorUnlock);
        jacob.gameObject.SetActive(doorUnlock);
        boss.gameObject.SetActive(false); // Il boss solo quando parla Stygian
        conversationManager = ConversationManager.Instance;
    }

    public void BossAppeared() // Invocato nel nodo del DE quando parla per la prima volta Stygian
    {
        boss.gameObject.SetActive(true);
        var bossAgent = boss.GetComponent<AIBossAgent>();
        bossAgent.enabled = false;
        bossAgent.AlignToPlayer();
        boss.GetComponent<AIBossLocomotion>().enabled = false;
        boss.GetComponent<Animator>().SetTrigger("entry");
        combinazioneManager.ToggleDoor(false); // La porta della zona boss si chiude
    }

    public void EndEntryBoss() // Invocato alla fine del dialogo del boss
    {
        boss.GetComponent<Animator>().SetTrigger("endEntry");
        boss.GetComponent<AIBossAgent>().enabled = true;
        boss.GetComponent<AIBossLocomotion>().enabled = true;
        gaia.GetComponent<AIGaiaNPC>().target = hideoutGaia;
    }

    public void MoveUpObj(GameObject obj) => StartCoroutine(MoveUpCoroutine(obj)); // Invocato nel nodo del DE quando JA parlano verso la fine del dialogo

    private void ToggleObj(GameObject obj, bool disable)
    {
        // Per il rigidbody
        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = disable;
            rb.useGravity = !disable;
        }

        // Per il NavMeshAgent
        obj.GetComponent<NavMeshAgent>().enabled = !disable;

        // Per l'Animator
        if (obj.TryGetComponent(out Animator animator))
        {
            animator.SetBool("isHanging", disable);
        }

        // Per il NPCAIWithWaypoints
        obj.GetComponent<NPCAIWithWaypoints>().SetWaypoints(newWaypoints);
    }

    IEnumerator MoveUpCoroutine(GameObject obj)
    {
        Vector3 startPosition = obj.transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, 4.25f, 0);
        float elapsedTime = 0f;
        float duration = 2f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            yield return null;
        }
        obj.transform.position = targetPosition;

        // Disabilita il rigidbody e il NavMeshAgent se no "obj" cade a terra
        ToggleObj(obj, true);
    }

    public void BossRecovery() => StartCoroutine(WaitForBossRecovery());

    IEnumerator WaitForBossRecovery()
    {
        // Se non è già stato liberato Jacob e Angelica
        if (!JacobAngelicaLiberi)
        {
            conversationManager.StartConversation(conversations[0]);
            JacobAngelicaLiberi = true;
        }
        var bossAgent = boss.GetComponent<AIBossAgent>();
        yield return new WaitForSeconds(7.5f);
        bossAgent.status.isStunned = false;
        bossAgent.animator.SetBool("isStunned", false);
        bossAgent.stateMachine.ChangeState(AIStateId.ChasePlayer);
    }

    public void UnlockFriends() // Invocata nel primo nodo del DE quando Stygian è stordito e gli amici vengono liberati
    {
        ToggleObj(angelica.gameObject, false);
        ToggleObj(jacob.gameObject, false);

        gaia.GetComponent<AIGaiaNPC>().enabled = false;
        gaia.GetComponent<Animator>().SetBool("crouching", false);
        var npc = gaia.GetComponent<NPCAIWithWaypoints>();
        npc.enabled = true;
        npc.SetWaypoints(newWaypoints);
    }
}
