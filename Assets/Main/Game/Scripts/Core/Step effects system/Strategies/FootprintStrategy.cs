using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
        var footprintSprites = _footprintData.GetFootprintSprites(context.SurfaceType);
        if (footprintSprites != null)
        {
            var footprint = _footprintPool.Get();
            footprint.transform.position = context.Position;
            footprint.transform.rotation = GetFootprintRotation(context.VelocityDirection);

            var randomSprite = footprintSprites[Random.Range(0, footprintSprites.Length)];
            footprint.SetSprite(randomSprite);
        }
    }

    private Quaternion GetFootprintRotation(Vector2 direction)
    {
        const float DefaultAngle = -90f;
        
        var angle = DefaultAngle;

        if (!Mathf.Approximately(direction.x, 0f) || !Mathf.Approximately(direction.y, 0f))
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        return Quaternion.Euler(0f, 0f, angle);
    }
}