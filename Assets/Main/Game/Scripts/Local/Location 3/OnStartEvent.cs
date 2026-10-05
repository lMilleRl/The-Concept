using System;
using UnityEngine;
using UnityEngine.Events;

public class OnStartEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent _onStarted;

    private void Start()
    {
        _onStarted?.Invoke();
    }
}
