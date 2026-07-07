using System;
using UnityEngine;
using UnityEngine.Playables;

public class PassiveShowActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayableDirector _director;
    [SerializeField] private bool _isReturningToGameplayInAnimationStopped = true;

    private void OnEnable()
    {
    }

    public void Activate()
    {
        GameStateManager.Instance.SetState(GameState.PassiveShow);
        _director.Play();
        if (_isReturningToGameplayInAnimationStopped)
            _director.stopped += ReturnToGameplay;
    }

    public void ReturnToGameplay() => ReturnToGameplay(_director);

    private void ReturnToGameplay(PlayableDirector director)
    {
        if (_isReturningToGameplayInAnimationStopped)
            _director.stopped -= ReturnToGameplay;
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }

    private void OnDestroy()
    {
        if (_director != null && _isReturningToGameplayInAnimationStopped)
            _director.stopped -= ReturnToGameplay;
    }
}