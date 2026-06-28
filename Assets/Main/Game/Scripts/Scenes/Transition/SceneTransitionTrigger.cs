using System;
using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TransitionData _transitionData;

    private void OnEnable()
    {
        
    }

    public void Activate()
    {
        TransitionHandler.Instance.StartTransition(_transitionData);
    }
}