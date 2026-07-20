using System;
using UnityEngine;

public abstract class StateTransition : IStateTransition
{
    private bool _isGlobal;

    protected IState DestinationState;
    
    private IState _fromThatStateActive;
    private IState _currentState;


    public StateTransition(BaseStateTransitionData data)
    {
        _fromThatStateActive = data.FromThatStateActive;
        DestinationState = data.DestinationState;
        _isGlobal = data.IsGlobal;
    }

    public bool IsGlobal => _isGlobal;

    public event Action<IState> ConditionCompleted;

    public bool TryToEnterState()
    {
        if (!_isGlobal)
        {
            if (IsCurrentStateThatForTransActive())
                return TryThrowRequestToEnterState();

            return false;
        }

        return TryThrowRequestToEnterState();
    }

    public void UpdateCurrentState(IState currentState)
    {
        _currentState = currentState;
    }

    protected abstract bool TryThrowRequestToEnterState();

    protected bool IsCurrentStateThatForTransActive()
    {
        return _currentState == _fromThatStateActive;
    }
    
    protected void ThrowThatConditionComplete()
    {
        ConditionCompleted?.Invoke(DestinationState);
    }
}