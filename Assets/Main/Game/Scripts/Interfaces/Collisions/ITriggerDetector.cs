using System;
using UnityEngine;

public interface ITriggerDetector
{
    event Action<Collider2D> Triggered;
    event Action<Collider2D> Exited;
}

