using UnityEngine;

public abstract class SurfaceProvider : MonoBehaviour, ISurfaceDetector
{
    public abstract SurfaceType GetSurface(Vector3 worldPosition);
}
