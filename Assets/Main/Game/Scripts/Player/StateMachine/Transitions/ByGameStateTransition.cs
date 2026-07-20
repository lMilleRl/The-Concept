using System;
using UnityEngine;

public class ByGameStateTransition : StateTransition
{
    private Predicate<GameState> _isNeededGameState;

    private IGameStateManager _gameStateManager;

    private bool _isNeedTransit;

    public ByGameStateTransition(BaseStateTransitionData data, IGameStateManager gameStateManager,
        Predicate<GameState> isNeededGameState) : base(data)
    {
        _gameStateManager = gameStateManager;
        _isNeededGameState = isNeededGameState;
        gameStateManager.OnStateChanged += HandleGameStateChange;
    }

    public event Action OnTransit;

    protected override bool TryThrowRequestToEnterState()
    {
        if (_isNeedTransit)
        {
            OnTransit?.Invoke();
            ThrowThatConditionComplete();
            _isNeedTransit = false;
            return true;
        }

        return false;
    }

    private void HandleGameStateChange(GameState newState)
    {
        _isNeedTransit = _isNeededGameState(newState);
    }

    ~ByGameStateTransition()
    {
        _gameStateManager.OnStateChanged -= HandleGameStateChange;
    }
}