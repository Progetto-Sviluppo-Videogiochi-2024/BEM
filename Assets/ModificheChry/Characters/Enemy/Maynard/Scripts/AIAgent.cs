using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    [Header("State Machine")]
    #region State Machine
    public AIStateId initialState; // Stato iniziale
    public AIStateMachine stateMachine; // Macchina a stati finitia
    #endregion

    [Header("References")]
    #region References
    public AI config; // Configurazione dell'IA
    public Collider patrolArea; // Area di pattugliamento
    public Player player; // Riferimento al giocatore
    [HideInInspector] public Rigidbody rb; // Riferimento al corpo rigidbody
    [HideInInspector] public NavMeshAgent navMeshAgent; // Riferimento all'agente di navigazione
    [HideInInspector] public AILocomotion locomotion; // Riferimento al componente di locomozione
    [HideInInspector] public AIStatus status; // Riferimento allo stato dell'IA
    [HideInInspector] public AIDetection detection; // Riferimento al componente di rilevamento
    [HideInInspector] public Animator animator; // Riferimento all'animatore
    #endregion

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        locomotion = GetComponent<AILocomotion>();
        status = GetComponent<AIStatus>();
        detection = GetComponent<AIDetection>();
        animator = GetComponent<Animator>();

        stateMachine = new(this);
        stateMachine.RegisterState(new AIPatrolState());
        stateMachine.RegisterState(new AIChasePlayerState());
        stateMachine.RegisterState(new AIAttackState());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.ChangeState(initialState);
    }

    void Update() => stateMachine.Update();
}
