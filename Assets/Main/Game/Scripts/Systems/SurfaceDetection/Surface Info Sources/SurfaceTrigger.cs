using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SurfaceTrigger : MonoBehaviour, ISurfaceInfo
{
    [SerializeField] private SurfaceType _surfaceType;

    public SurfaceType SurfaceType => _surfaceType;
}
