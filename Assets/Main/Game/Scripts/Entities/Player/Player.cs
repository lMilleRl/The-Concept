using System;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInteraction))]
public class Player : MonoBehaviour
{
    private PlayerMovement _movement;
    private PlayerInteraction _interaction;

    private void Awake()
    {
        _movement = GetComponent<PlayerMovement>();
        _interaction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        GameStateManager.Instance.OnStateChanged += TryChangeUpdate;
        TryChangeUpdate(GameStateManager.Instance.CurrentState);
    }
    
    private void OnDisable()
    {
        GameStateManager.Instance.OnStateChanged -= TryChangeUpdate;
    }

    private void TryChangeUpdate(GameState state)
    {
        if (state == GameState.Gameplay)
            ResumeUpdate();
        else
            StopUpdate();
    }

    public void StopUpdate()
    {
        _movement.enabled = false;
        _interaction.enabled = false;
    }

    public void ResumeUpdate()
    {
        _movement.enabled = true;
        _interaction.enabled = true;
    }
}