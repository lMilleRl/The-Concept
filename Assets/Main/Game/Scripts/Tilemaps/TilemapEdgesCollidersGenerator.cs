using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapEdgesCollidersGenerator : MonoBehaviour
{
    // Сюда в инспекторе перетащите любой невидимый или технический тайл
    [SerializeField] private TileBase _colliderTile;

    // Перетащите сюда ваш основной тайлмап с землей из иерархии
    [SerializeField] private Tilemap _visualTilemap;

    private Tilemap _colliderTilemap;


    [ContextMenu("Generate Colliders in edges")]
    public void GenerateInEdges()
    {
        Generate(IsEdge);
    }

    [ContextMenu("Generate Colliders beyond edges")]
    public void GenerateBeyondEdges()
    {
        Generate(IsBeyondEdge);
    }

    private void Generate(Predicate<Vector3Int> conditionToSetColliderInCell)
    {
        _colliderTilemap = GetComponent<Tilemap>();
        _colliderTilemap.ClearAllTiles(); // Очищаем старые границы

        // Пробегаем по всем заполненным клеткам видимой земли
        _visualTilemap.CompressBounds();
        BoundsInt bounds = _visualTilemap.cellBounds;
        for (int x = bounds.xMin - 1; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin - 1; y <= bounds.yMax; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                // Если это крайняя клетка (у неё есть пустые соседи)
                if (!IsInnerTile(pos) && conditionToSetColliderInCell(pos))
                {
                    _colliderTilemap.SetTile(pos, _colliderTile);
                }
            }
        }
    }

    private bool IsInnerTile(Vector3Int pos)
    {
        return _visualTilemap.HasTile(pos + Vector3Int.up) &&
               _visualTilemap.HasTile(pos + Vector3Int.down) &&
               _visualTilemap.HasTile(pos + Vector3Int.left) &&
               _visualTilemap.HasTile(pos + Vector3Int.right) &&
               _visualTilemap.HasTile(pos + Vector3Int.left + Vector3Int.up) &&
               _visualTilemap.HasTile(pos + Vector3Int.right + Vector3Int.up) &&
               _visualTilemap.HasTile(pos + Vector3Int.left + Vector3Int.down) &&
               _visualTilemap.HasTile(pos + Vector3Int.right + Vector3Int.down) &&
               _visualTilemap.HasTile(pos);
    }
    
    private bool IsEdge(Vector3Int pos)
    {
        return (!_visualTilemap.HasTile(pos + Vector3Int.up) ||
               !_visualTilemap.HasTile(pos + Vector3Int.down) ||
               !_visualTilemap.HasTile(pos + Vector3Int.right) ||
               !_visualTilemap.HasTile(pos + Vector3Int.left) ||
               !_visualTilemap.HasTile(pos + Vector3Int.left + Vector3Int.up) ||
               !_visualTilemap.HasTile(pos + Vector3Int.right + Vector3Int.up) ||
               !_visualTilemap.HasTile(pos + Vector3Int.left + Vector3Int.down) ||
               !_visualTilemap.HasTile(pos + Vector3Int.right + Vector3Int.down)) &&
               _visualTilemap.HasTile(pos);
    }

    private bool IsBeyondEdge(Vector3Int pos)
    {
        return (_visualTilemap.HasTile(pos + Vector3Int.up) ||
                _visualTilemap.HasTile(pos + Vector3Int.down) ||
                _visualTilemap.HasTile(pos + Vector3Int.left) ||
                _visualTilemap.HasTile(pos + Vector3Int.right)) &&
               !_visualTilemap.HasTile(pos);
    }
}