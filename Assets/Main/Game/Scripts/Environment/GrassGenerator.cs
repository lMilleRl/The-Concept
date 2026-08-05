#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GrassGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private Vector2 _areaSize = new Vector2(20f, 10f);
    [SerializeField] private float _minSpacing = 0.5f;
    [SerializeField][Range(0f, 1f)] private float _density = 0.8f;
    [SerializeField][Min(1)] private int _maxRetries = 5;
    [SerializeField] private int _seed;
    [SerializeField] private bool _randomScale = true;
    [SerializeField] private Vector2 _scaleRange = new Vector2(0.8f, 1.2f);
    [SerializeField] private bool _randomRotation;
    [SerializeField] private float _rotationRange = 15f;

    [SerializeField][HideInInspector] private List<Vector2> _generatedPositions = new List<Vector2>();

    private const float SpacingLowerBound = 0.001f;
    private const int NeighborSearchRadius = 1;
    private const int MaxPrefabPickAttempts = 10;

    private Transform _generatedRoot;

    private GameObject GetRandomPrefab()
    {
        if (_prefabs == null || _prefabs.Length == 0)
            return null;

        for (int i = 0; i < MaxPrefabPickAttempts; i++)
        {
            GameObject prefab = _prefabs[Random.Range(0, _prefabs.Length)];
            if (prefab != null)
                return prefab;
        }

        return _prefabs[0];
    }

    [ContextMenu("Generate")]
    public void Generate()
    {
#if UNITY_EDITOR
        Undo.SetCurrentGroupName("Generate Grass");
#endif
        Clear();

        if (_prefabs == null || _prefabs.Length == 0 || _prefabs[0] == null)
        {
            Debug.LogWarning("At least one grass prefab must be assigned.", this);
            return;
        }

        Random.InitState(_seed);

        float spacing = Mathf.Max(_minSpacing, SpacingLowerBound);
        float cellSize = spacing;
        Vector2 center = transform.position;
        Vector2 halfSize = _areaSize * 0.5f;
        Vector2 min = center - halfSize;

        int columns = Mathf.Max(1, Mathf.CeilToInt(_areaSize.x / cellSize));
        int rows = Mathf.Max(1, Mathf.CeilToInt(_areaSize.y / cellSize));

        var grid = new List<Vector2>[columns, rows];
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
                grid[x, y] = new List<Vector2>();

        _generatedPositions.Clear();

        _generatedRoot = new GameObject("Generated Grass").transform;
        _generatedRoot.SetParent(transform, false);
        _generatedRoot.localPosition = Vector3.zero;

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(_generatedRoot.gameObject, "Generate Grass");
#endif

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (_density < 1f && Random.value > _density)
                    continue;

                Vector2 cellMin = min + new Vector2(x * cellSize, y * cellSize);
                TryPlaceInCell(cellMin, cellSize, spacing, grid, columns, rows);
            }
        }
    }

    private void TryPlaceInCell(Vector2 cellMin, float cellSize, float spacing, List<Vector2>[,] grid, int columns, int rows)
    {
        for (int attempt = 0; attempt < _maxRetries; attempt++)
        {
            Vector2 point = new Vector2(
                Random.Range(cellMin.x, cellMin.x + cellSize),
                Random.Range(cellMin.y, cellMin.y + cellSize));

            if (!IsValid(point, grid, columns, rows, spacing))
                continue;

            Place(point, grid, columns, rows);
            return;
        }
    }

    private bool IsValid(Vector2 point, List<Vector2>[,] grid, int columns, int rows, float spacing)
    {
        Vector2 min = (Vector2)transform.position - _areaSize * 0.5f;
        float cellSize = spacing;
        int cx = Mathf.FloorToInt((point.x - min.x) / cellSize);
        int cy = Mathf.FloorToInt((point.y - min.y) / cellSize);
        cx = Mathf.Clamp(cx, 0, columns - 1);
        cy = Mathf.Clamp(cy, 0, rows - 1);

        float sqrSpacing = spacing * spacing;

        for (int dx = -NeighborSearchRadius; dx <= NeighborSearchRadius; dx++)
        {
            int nx = cx + dx;
            if (nx < 0 || nx >= columns)
                continue;

            for (int dy = -NeighborSearchRadius; dy <= NeighborSearchRadius; dy++)
            {
                int ny = cy + dy;
                if (ny < 0 || ny >= rows)
                    continue;

                foreach (Vector2 neighbor in grid[nx, ny])
                {
                    if ((point - neighbor).sqrMagnitude < sqrSpacing)
                        return false;
                }
            }
        }

        return true;
    }

    private void Place(Vector2 point, List<Vector2>[,] grid, int columns, int rows)
    {
        Vector2 min = (Vector2)transform.position - _areaSize * 0.5f;
        float cellSize = _minSpacing;
        int cx = Mathf.FloorToInt((point.x - min.x) / cellSize);
        int cy = Mathf.FloorToInt((point.y - min.y) / cellSize);
        cx = Mathf.Clamp(cx, 0, columns - 1);
        cy = Mathf.Clamp(cy, 0, rows - 1);

        grid[cx, cy].Add(point);
        _generatedPositions.Add(point);

        Vector3 position = new Vector3(point.x, point.y, transform.position.z);
        Quaternion rotation = _randomRotation
            ? Quaternion.Euler(0f, 0f, Random.Range(-_rotationRange, _rotationRange))
            : Quaternion.identity;
        Vector3 scale = _randomScale
            ? Vector3.one * Random.Range(_scaleRange.x, _scaleRange.y)
            : Vector3.one;

        GameObject prefab = GetRandomPrefab();
        if (prefab == null)
            return;

        GameObject instance;
#if UNITY_EDITOR
        instance = PrefabUtility.InstantiatePrefab(prefab, _generatedRoot) as GameObject;
        if (instance == null)
            instance = Instantiate(prefab, _generatedRoot);
#else
        instance = Instantiate(prefab, _generatedRoot);
#endif

        instance.transform.position = position;
        instance.transform.localRotation = rotation;
        instance.transform.localScale = scale;
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        if (_generatedRoot != null)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(_generatedRoot.gameObject);
#else
            DestroyImmediate(_generatedRoot.gameObject);
#endif
        }
        else
        {
            Transform child = transform.Find("Generated Grass");
            if (child != null)
            {
#if UNITY_EDITOR
                Undo.DestroyObjectImmediate(child.gameObject);
#else
                DestroyImmediate(child.gameObject);
#endif
            }
        }

        _generatedRoot = null;
        _generatedPositions.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
        Gizmos.DrawWireCube(transform.position, new Vector3(_areaSize.x, _areaSize.y, 0.1f));

        if (_generatedPositions == null || _generatedPositions.Count == 0)
            return;

        Gizmos.color = new Color(0.4f, 1f, 0.4f, 0.5f);
        float radius = Mathf.Max(0.02f, _minSpacing * 0.1f);
        foreach (Vector2 position in _generatedPositions)
        {
            Vector3 worldPosition = new Vector3(position.x, position.y, transform.position.z);
            Gizmos.DrawCube(worldPosition, Vector3.one * radius);
        }
    }
}
