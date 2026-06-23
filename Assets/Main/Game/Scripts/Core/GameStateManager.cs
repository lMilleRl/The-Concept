using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour, IGameStateManager
{
    public static IGameStateManager Instance;

    private GameState _currentState;

    public GameState CurrentState => _currentState;
    
    public event Action<GameState> OnStateChanged;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    public void SetState(GameState state)
    {
        _currentState = state;
        OnStateChanged?.Invoke(state);
    }
}
