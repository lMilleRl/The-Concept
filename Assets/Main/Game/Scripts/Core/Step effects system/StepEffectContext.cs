using UnityEngine;

public readonly struct StepEffectContext
{
    public readonly SurfaceType SurfaceType;
    public readonly Vector3 Position;
    public readonly Vector2 VelocityDirection;
    public readonly Foot Foot;
    public readonly bool IsStop;

    public StepEffectContext(SurfaceType surfaceType, Vector3 position, Vector2 velocityDirection, Foot foot, bool isStop)
    {
        SurfaceType = surfaceType;
        Position = position;
        VelocityDirection = velocityDirection;
        Foot = foot;
        IsStop = isStop;
    }
}
