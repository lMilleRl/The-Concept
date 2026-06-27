using UnityEngine;
using UnityEngine.Playables;

public class PassiveShowActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private PlayableDirector _director;

    private void OnEnable()
    {
        _director.stopped += OnDirectorStopped;
    }

    private void OnDisable()
    {
        _director.stopped -= OnDirectorStopped;
    }

    public void Activate()
    {
        GameStateManager.Instance.SetState(GameState.PassiveShow);
        _director.Play();
    }

    private void OnDirectorStopped(PlayableDirector director)
    {
        GameStateManager.Instance.SetState(GameState.Gameplay);
    }
}
