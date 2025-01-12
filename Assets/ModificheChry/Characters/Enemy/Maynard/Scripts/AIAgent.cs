using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    public AIStateMachine stateMachine;
    public AIStateId initialState;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public AILocomotion locomotion;
    public AI config;
    public Collider patrolArea;
    public Transform player; // Riferimento al giocatore

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        locomotion = GetComponent<AILocomotion>();
        stateMachine = new(this);
        stateMachine.RegisterState(new AIPatrolState());
        stateMachine.RegisterState(new AIChasePlayerState());
        stateMachine.RegisterState(new AIAttackState());
        stateMachine.RegisterState(new AIDeathState());
        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
    }
}
