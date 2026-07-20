public class FromPassiveStateTransition : StateTransition
{
    private IStateMachine _stateMachine;
    private IGameStateManager _gameStateManager;

    public FromPassiveStateTransition(BaseStateTransitionData data, IStateMachine currentStateMachine,
        ByGameStateTransition enterTrans, IGameStateManager gameStateManager) : base(data)
    {
        _stateMachine = currentStateMachine;
        enterTrans.OnTransit += SetPreviousStateAsDestination;
        _gameStateManager = gameStateManager;
    }

    private void SetPreviousStateAsDestination()
    {
        DestinationState = _stateMachine.CurrentState;
    }

    protected override bool TryThrowRequestToEnterState()
    {
        if (_gameStateManager.CurrentState == GameState.Gameplay)
        {
            ThrowThatConditionComplete();
            return true;
        }

        return false;
    }
}