using UnityEngine;

public class StateMachine : IStateMachine
{
    private IState[] _states;
    private IState _currentState;

    public IState CurrentState => _currentState;

    public StateMachine(IState initState, IState[] states)
    {
        _currentState = initState;
        _states = states;
    }
    
    public void Update()
    {
        _currentState?.Update();
    }

    public void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    public void SetNextState(IState nextState)
    {
        _currentState?.Exit();
        _currentState = nextState;
        _currentState?.Enter();
    }
}