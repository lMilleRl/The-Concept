using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "SurfaceTile", menuName = "Tiles/Surface Tile")]
public class SurfaceTile : Tile, ISurfaceInfo
{
    [SerializeField] private SurfaceType _surfaceType;

    public SurfaceType SurfaceType => _surfaceType;
}
