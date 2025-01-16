using System.Collections;
using DialogueEditor;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("Settings")]
    #region Settings
    [HideInInspector] public string id = ""; // ID dell'IA: utile nel caricamento dei dati per capire se è stato ucciso 
    #endregion

    [Header("State Machine")]
    #region State Machine
    public AIStateId initialState; // Stato iniziale
    public AIStateMachine stateMachine; // Macchina a stati finitia
    #endregion

    [Header("Config AI")]
    #region Config AI
    public float minDistanceAttack = 2f; // Distanza minima per attaccare il giocatore
    public float attackCooldown = 2.5f; // Tempo di cooldown tra un attacco e l'altro
    #endregion

    [Header("SFX")]
    #region SFX
    [SerializeField] private AudioClip[] soundsAI; // Suoni dell'IA: 0 = non triggerato, 1 = triggerato, 2 = insegue, 3 = morte
    [HideInInspector] public AudioSource audioSource; // Riferimento all'audio source
    #endregion

    [Header("Conversations")]
    #region Conversations
    public string nameBoolBA; // Nome del boolean accessor da settare
    public NPCConversation conversation; // Conversazione che si avvia quando il giocatore uccide il mutante
    #endregion

    [Header("AI References")]
    #region References
    public Collider patrolArea; // Area di pattugliamento
    public Player player; // Riferimento al giocatore
    [HideInInspector] public NavMeshAgent navMeshAgent; // Riferimento all'agente di navigazione
    [HideInInspector] public AILocomotion locomotion; // Riferimento al componente di locomozione
    [HideInInspector] public AIStatus status; // Riferimento allo stato dell'IA
    [HideInInspector] public AIDetection detection; // Riferimento al componente di rilevamento
    [HideInInspector] public Animator animator; // Riferimento all'animatore
    [SerializeField] public LayerMask layerMask; // LayerMask per il rilevamento e gli attacchi
    [HideInInspector] public IAttackAI mutantAttack; // Riferimento al componente script dell'attacco di ogni mutante
    [HideInInspector] public RagdollManager ragdollManager; // Riferimento al componente di gestione del ragdoll
    #endregion

    void Start()
    {
        // Al caricamento della scena, se l'agente è stato ucciso precedentemente, disattivalo
        if (GestoreScena.killedEnemyIds.Contains(id)) { gameObject.SetActive(false); return; }

        navMeshAgent = GetComponent<NavMeshAgent>();
        locomotion = GetComponent<AILocomotion>();
        status = GetComponent<AIStatus>();
        detection = GetComponent<AIDetection>();
        animator = GetComponent<Animator>();
        mutantAttack = GetComponent<IAttackAI>();
        ragdollManager = GetComponent<RagdollManager>();
        audioSource = GetComponent<AudioSource>();
        PlayAudio(0, true);

        stateMachine = new(this);
        stateMachine.RegisterState(new AIPatrolState());
        stateMachine.RegisterState(new AIChasePlayerState());
        stateMachine.RegisterState(new AIAttackState());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.ChangeState(initialState);
    }

    void Update() => stateMachine?.Update();

    public void PlayAudio(int index, bool loop)
    {
        StopAudio();
        audioSource.loop = loop;
        audioSource.clip = soundsAI[index];
        audioSource.Play();
    }

    public void StopAudio() => audioSource.Stop();

    public IEnumerator PlayNextAudio(int index)
    {
        if (!status.IsEnemyAlive()) yield break; // Interrompe immediatamente la coroutine
        PlayAudio(index, false);

        yield return new WaitForSeconds(audioSource.clip.length);

        if (!status.IsEnemyAlive()) yield break; // Interrompe immediatamente la coroutine
        PlayAudio(index + 1, true);
    }

    public void StartConversation()
    {
        GestoreScena.ChangeCursorActiveStatus(true, "DeathState.AiAgent.StartConversation: " + gameObject.name);
        ConversationManager.Instance.StartConversation(conversation);
        ConversationManager.OnConversationEnded += OnDialogueEnded;
    }

    void OnDialogueEnded()
    {
        GestoreScena.ChangeCursorActiveStatus(false, "AiAgent.OnDialogueEnded: " + gameObject.name);
        BooleanAccessor.istance.SetBoolOnDialogueE(nameBoolBA);
        ConversationManager.OnConversationEnded -= OnDialogueEnded;
    }
}
