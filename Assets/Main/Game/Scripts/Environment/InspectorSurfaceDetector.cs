using UnityEngine;

public class InspectorSurfaceDetector : SurfaceProvider
{
    [SerializeField] private SurfaceType _defaultSurface;

    public override SurfaceType GetSurface(Vector3 worldPosition) => _defaultSurface;
}
