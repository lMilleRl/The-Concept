using UnityEngine;

public class InspectorSurfaceDetector : MonoBehaviour, ISurfaceDetector
{
    [SerializeField] private SurfaceType _defaultSurface;

    public SurfaceType GetSurface(Vector3 worldPosition) => _defaultSurface;
}
