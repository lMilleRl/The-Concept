using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SceneTransitionTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private TransitionData _transitionData;

    public void Activate()
    {
        TransitionHandler.Instance.StartTransition(_transitionData);
    }
}