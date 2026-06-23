using System;

public interface IGameStateManager
{
    GameState CurrentState { get; }
    
    event Action<GameState> OnStateChanged;

    void SetState(GameState state);
}
