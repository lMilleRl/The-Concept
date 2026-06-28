using System;
using UnityEngine;
using UnityEngine.Events;

public class InteractableEvent : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _onActivate;

    private void OnEnable()
    {
        
    }

    public void Activate()
    {
        _onActivate?.Invoke();
    }
}
