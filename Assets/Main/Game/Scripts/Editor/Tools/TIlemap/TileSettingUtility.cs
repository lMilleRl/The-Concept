using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSettingUtility : EditorWindow
{
    private Tilemap _source;
    private Tilemap _destination;
    private Tile _tileForSet;
    
    [MenuItem("Tools/Tiles Setting")]
    public static void OpenWindow()
    {
        GetWindow<TileSettingUtility>("Tiles Setting");
    }

    private void OnGUI()
    {
        _source = (Tilemap)EditorGUILayout.ObjectField("Source", _source, typeof(Tilemap), true);
        _destination = (Tilemap)EditorGUILayout.ObjectField("Destination", _destination, typeof(Tilemap), true);
        _tileForSet = (Tile)EditorGUILayout.ObjectField("Tile For Set", _tileForSet, typeof(Tile), false);

        if (GUILayout.Button("Set Tiles in positions of the source"))
        {
            SetFromSource();
        }
        
        if (GUILayout.Button("Clear Destination"))
        {
            ClearDestination();
        }
    }

    private void SetFromSource()
    {
        if (!IsInputDataValid())
            return;

        Undo.RecordObject(_destination, "Set From Source");
        
        var tiles = new List<TileBase>();
        var positions = new List<Vector3Int>();

        foreach (var tilePos in _source.cellBounds.allPositionsWithin)
        {
            if (_source.GetTile(tilePos) == null)
                continue;

            positions.Add(tilePos);
            tiles.Add(_tileForSet);
        }

        _destination.SetTiles(positions.ToArray(), tiles.ToArray());
        
        _destination.RefreshAllTiles();
        EditorUtility.SetDirty(_destination);
    }

    private void ClearDestination()
    {
        if (!IsInputDataValid())
            return;

        Undo.RecordObject(_destination, "Clear Destination");
        
        foreach (var tilePos in _source.cellBounds.allPositionsWithin)
        {
            if (_source.GetTile(tilePos) == null)
                continue;

            _destination.SetTile(tilePos, null);
        }
        
        _destination.RefreshAllTiles();
        EditorUtility.SetDirty(_destination);
    }

    private bool IsInputDataValid()
    {
        return _source != null && _destination != null && _tileForSet != null;
    }
}
