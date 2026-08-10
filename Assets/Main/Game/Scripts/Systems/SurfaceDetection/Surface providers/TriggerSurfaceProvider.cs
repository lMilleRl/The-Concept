using System.Collections.Generic;
using UnityEngine;

public class TriggerSurfaceProvider : SurfaceProvider
{
    [SerializeField] private LayerMask _surfaceLayerMask;

    private readonly List<Collider2D> _results = new List<Collider2D>(8);
    private ContactFilter2D _filter;
    
    private void Awake()
    {
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_surfaceLayerMask);
        _filter.useTriggers = true;
    }

    
    public override SurfaceType GetSurface(Vector3 worldPosition)
    {
        _results.Clear();
        int count = Physics2D.OverlapPoint(worldPosition, _filter, _results);

        for (int i = 0; i < count; i++)
        {
            if (_results[i].TryGetComponent(out ISurfaceInfo surfaceInfo))
                return surfaceInfo.SurfaceType;
        }

        return SurfaceType.None;
    }
}
