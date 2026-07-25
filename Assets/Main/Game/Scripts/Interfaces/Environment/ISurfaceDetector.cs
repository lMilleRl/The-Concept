using UnityEngine;

public interface ISurfaceDetector
{
    SurfaceType GetSurface(Vector3 worldPosition);
}
