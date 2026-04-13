using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneTransitionTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private SceneTransitionData _transitionData;

    public static event Action<SceneTransitionData> OnTriggerActivated;
    
    public void Activate()
    {
        OnTriggerActivated?.Invoke(_transitionData);
    }
}