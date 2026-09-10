using UnityEngine;

public interface IPlayerMovement
{
    Vector2 Velocity { get; }
    Vector2 IntendedDirection { get; }
    float NormalizedSpeed { get; }
    bool IsMovingByInput { get; }
    void SetInput(IMoveInput input);
}
