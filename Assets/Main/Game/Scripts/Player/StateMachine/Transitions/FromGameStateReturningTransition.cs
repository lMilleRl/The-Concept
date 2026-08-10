public class FromGameStateReturningTransition : StateTransition
{
    private IStateMachine _stateMachine;
    private IGameStateManager _gameStateManager;
    private GameState _gameStateForReturningToPreviousState;
    
    public FromGameStateReturningTransition(BaseStateTransitionData data, IStateMachine currentStateMachine,
        ByGameStateTransition dependenceEnterTrans, IGameStateManager gameStateManager, GameState stateForReturning) : base(data)
    {
        _stateMachine = currentStateMachine;
        dependenceEnterTrans.OnTransit += SetPreviousStateAsDestination;
        _gameStateManager = gameStateManager;
        _gameStateForReturningToPreviousState = stateForReturning;
    }

    private void SetPreviousStateAsDestination()
    {
        DestinationState = _stateMachine.CurrentState;
    }

    protected override bool TryThrowRequestToEnterState()
    {
        if (_gameStateManager.CurrentState == _gameStateForReturningToPreviousState)
        {
            ThrowThatConditionComplete();
            return true;
        }

        return false;
    }
}