using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BodyCollisionsDetector : MonoBehaviour, ITriggerDetector
{
    public event Action<Collider2D> Triggered;
    public event Action<Collider2D> Exited;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Triggered?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Exited?.Invoke(other);
    }
}

