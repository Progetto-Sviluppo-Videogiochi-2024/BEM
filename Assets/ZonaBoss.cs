using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using DialogueEditor;

public class ZonaBoss : MonoBehaviour
{
    [Header("Riferimenti-Tranform")]
    #region Riferimenti
    public Transform characters; // Riferimento ai personaggi della zona boss
    public Transform gaia; // Riferimento a Gaia
    private Transform angelica; // Riferimento ad Angelica
    private Transform jacob; // Riferimento a Jacob
    private Transform boss; // Riferimento al boss
    public CombinazioneManager combinazioneManager; // Riferimento al CombinazioneManager
    #endregion

    [Header("Conversations")]
    #region Conversations
    public NPCConversation[] conversations;
    private ConversationManager conversationManager;
    private bool JacobAngelicaLiberi = false; // Flag per riprodurre la conversazione una sola volta
    #endregion

    //Le missioni le stiamo gestendo dai due dialoghi Riunione&Boss e BossStunned  

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
    }

    public void MoveUpObj(GameObject obj) => StartCoroutine(MoveUpCoroutine(obj)); // Invocato nel nodo del DE quando JA parlano verso la fine del dialogo

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
        var rb = obj.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        obj.GetComponent<NavMeshAgent>().enabled = false;
        var animator = obj.GetComponent<Animator>();
        animator.SetTrigger("hanging");
        animator.SetBool("isHanging", true);
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
}
