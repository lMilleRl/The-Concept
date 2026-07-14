using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
public class PlayerInputActiveHandler : MonoBehaviour
{
    private IPlayerInput[] _inputs;

    public void Init(IPlayerInput[] inputs)
    {
        _inputs = inputs;
    }

    private void Start()
    {
        SubscribeToGameplayState();
    }

    private void OnEnable()
    {
        if (GameStateManager.Instance == null) return;
        SubscribeToGameplayState();
    }

    private void OnDisable()
    {
        if (GameStateManager.Instance == null) return;
        GameStateManager.Instance.OnStateChanged -= TryChangeUpdate;
    }

    private void SubscribeToGameplayState()
    {
        GameStateManager.Instance.OnStateChanged += TryChangeUpdate;
        TryChangeUpdate(GameStateManager.Instance.CurrentState);
    }

    private void TryChangeUpdate(GameState state)
    {
        if (state == GameState.Gameplay)
            ResumeInputs();
        else
            StopInputs();
    }

    public void StopInputs()
    {
        foreach (var i in _inputs)
        {
            i.IsInputEnabled = false;
        }
    }

    public void ResumeInputs()
    {
        foreach (var i in _inputs)
        {
            i.IsInputEnabled = true;
        }
    }
}