using UnityEngine;

public class SurfaceDetector : MonoBehaviour, ISurfaceDetector
{
    [Tooltip("Providers are checked in order. First provider that returns a non-None surface wins.")]
    [SerializeField] private SurfaceProvider[] _providers;
    [SerializeField] private SurfaceType _fallback = SurfaceType.None;

    public SurfaceType GetSurface(Vector3 worldPosition)
    {
        if (_providers == null)
            return _fallback;

        foreach (SurfaceProvider provider in _providers)
        {
            if (provider == null)
                continue;

            SurfaceType surface = provider.GetSurface(worldPosition);
            if (surface != SurfaceType.None)
                return surface;
        }

        return _fallback;
    }
}
