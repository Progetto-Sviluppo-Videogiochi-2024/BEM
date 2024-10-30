
public class AIStateMachine
{
    public AIState[] states;
    AIAgent agent;
    public AIStateId currentState;

    public AIStateMachine(AIAgent agent)
    {
        this.agent = agent;
        int nStates = System.Enum.GetNames(typeof(AIStateId)).Length;
        states = new AIState[nStates];
    }

    public void RegisterState(AIState state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AIState GetState(AIStateId stateId) => states[(int)stateId];

    public void Update() => GetState(currentState)?.Update(agent);

    public void ChangeState(AIStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }
}
