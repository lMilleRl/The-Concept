using UnityEngine;
using UnityEngine.Events;

public class InteractableEvent : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent _onActivate;

    public void Activate()
    {
        _onActivate?.Invoke();
    }
}
