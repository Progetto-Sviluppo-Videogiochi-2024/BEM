
public enum AIStateId
{
    Patrol,
    ChasePlayer,
    Attack, 
    Death
}

public interface AIState
{
    AIStateId GetId();
    void Enter(AIAgent agent);
    void Update(AIAgent agent);
    void Exit(AIAgent agent);
}
