using System;

public class AIStateMachine<TAgent> where TAgent : class
{
    public AIState<TAgent>[] states; // Array di stati della macchina a stati
    private TAgent agent; // Riferimento all'agente (generico)
    public AIStateId currentState; // Stato corrente della macchina a stati

    public AIStateMachine(TAgent agent)
    {
        this.agent = agent;
        int nStates = Enum.GetNames(typeof(AIStateId)).Length;
        states = new AIState<TAgent>[nStates];
    }

    public void RegisterState(AIState<TAgent> state)
    {
        int index = (int)state.GetId();
        states[index] = state;
    }

    public AIState<TAgent> GetState(AIStateId stateId) => states[(int)stateId];

    public void Update() => GetState(currentState)?.Update(agent);

    public void ChangeState(AIStateId newState)
    {
        GetState(currentState)?.Exit(agent);
        currentState = newState;
        GetState(currentState)?.Enter(agent);
    }

    public static implicit operator AIStateMachine<TAgent>(AIStateMachine<AIAgent> v)
    {
        throw new NotImplementedException();
    }
}
