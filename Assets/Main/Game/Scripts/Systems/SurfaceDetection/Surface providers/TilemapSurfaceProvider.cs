using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSurfaceProvider : SurfaceProvider
{
    [SerializeField] private Tilemap _surfaceTilemap;

    public override SurfaceType GetSurface(Vector3 worldPosition)
    {
        if (_surfaceTilemap == null)
            return SurfaceType.None;

        Vector3Int cellPos = _surfaceTilemap.WorldToCell(worldPosition);
        TileBase tile = _surfaceTilemap.GetTile(cellPos);

        if (tile is ISurfaceInfo surfaceInfo)
            return surfaceInfo.SurfaceType;

        return SurfaceType.None;
    }
}
