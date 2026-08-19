using UnityEngine;

public interface IAnimationMovementSource
{
    Vector2 IntendedDirection { get; }
    Vector2 ActualVelocity { get; }
}
