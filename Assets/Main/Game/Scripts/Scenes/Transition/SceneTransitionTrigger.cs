using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TransitionData _transitionData;

    public void Activate()
    {
        TransitionHandler.Instance.StartTransition(_transitionData);
    }
}