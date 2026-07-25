using UnityEngine;

public readonly struct StepEffectContext
{
    public readonly SurfaceType SurfaceType;
    public readonly Vector3 Position;
    public readonly Vector2 VelocityDirection;

    public StepEffectContext(SurfaceType surfaceType, Vector3 position, Vector2 velocityDirection)
    {
        SurfaceType = surfaceType;
        Position = position;
        VelocityDirection = velocityDirection;
    }
}