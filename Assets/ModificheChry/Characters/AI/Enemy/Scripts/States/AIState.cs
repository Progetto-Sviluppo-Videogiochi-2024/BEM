
public enum AIStateId
{
    Patrol,
    ChasePlayer,
    Attack, 
    Death
}

public interface AIState<TAgent>
{
    AIStateId GetId();
    void Enter(TAgent agent);
    void Update(TAgent agent);
    void Exit(TAgent agent);
}
