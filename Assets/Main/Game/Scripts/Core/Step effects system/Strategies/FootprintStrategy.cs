using System;
using UnityEngine;

public class FootprintStrategy : IStepEffectStrategy
{
    private FootprintData _footprintData;
    private GameObjectPool<Footprint> _footprintPool;

    public FootprintStrategy(FootprintData footprintData, Footprint prefab)
    {
        _footprintData = footprintData;
        _footprintPool = new GameObjectPool<Footprint>(prefab);
    }

    public void Execute(StepEffectContext context)
    {
        var direction = context.VelocityDirection;
        if (direction.sqrMagnitude < 1e-8f)
            direction = Vector2.down;

        if (context.IsStop)
        {
            Place(Foot.Left, context, direction);
            Place(Foot.Right, context, direction);
        }
        else
        {
            Place(context.Foot, context, direction);
        }
    }

    private void Place(Foot foot, StepEffectContext context, Vector2 direction)
    {
        if (!_footprintData.TryGetFootprintSprite(context.SurfaceType, foot, out var sprite, out var isMirrored))
            return;

        var perpendicular = new Vector2(-direction.y, direction.x);
        var side = foot == Foot.Left ? 1f : -1f;
        var position = context.Position + (Vector3)(perpendicular * side * _footprintData.FeetSpacing);
        var rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - _footprintData.SpriteForwardAngle);

        var footprint = _footprintPool.Get();
        footprint.transform.position = position;
        footprint.transform.rotation = rotation;
        footprint.SetSprite(sprite, isMirrored);
    }
}
