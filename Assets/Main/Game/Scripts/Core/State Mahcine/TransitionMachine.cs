using UnityEngine;

public class TransitionMachine : ITransitionMachine
{
    private IStateMachine _stateMachine;
    private StateTransition[] _transitions;

    public TransitionMachine(IStateMachine stateMachine, StateTransition[] transitions)
    {
        _stateMachine = stateMachine;
        _transitions = transitions;
        foreach (var t in _transitions)
        {
            if (t != null) t.ConditionCompleted += RequestTransit;
        }
    }

    public void UpdateTransitions()
    {
        foreach (var t in _transitions)
        {
            t.UpdateCurrentState(_stateMachine.CurrentState);
            if (t.TryToEnterState())
                break;
        }
    }

    private void RequestTransit(IState state)
    {
        _stateMachine.SetNextState(state);
    }

    ~TransitionMachine()
    {
        foreach (var t in _transitions)
        {
            if (t != null) t.ConditionCompleted -= RequestTransit;
        }
    }
}