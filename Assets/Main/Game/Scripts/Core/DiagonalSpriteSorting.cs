using System.Collections.Generic;
using UnityEngine;

public class DiagonalSpriteSorting : MonoBehaviour
{
    [SerializeField] private Transform _linePointA;
    [SerializeField] private Transform _linePointB;
    [SerializeField] private int _sortingOrderOffset = 1;

    private Dictionary<SpriteRenderer, int> _trackedObjects = new Dictionary<SpriteRenderer, int>();

    private void Update()
    {
        if (_trackedObjects.Count == 0) return;

        Vector2 lineDir = (Vector2)(_linePointB.position - _linePointA.position);
        Vector2 lineOrigin = _linePointA.position;

        foreach (var pair in _trackedObjects)
        {
            float side = GetSide(lineDir, lineOrigin, pair.Key.transform.position);

            pair.Key.sortingOrder = side > 0
                ? pair.Value - _sortingOrderOffset
                : pair.Value + _sortingOrderOffset;
        }
    }

    private float GetSide(Vector2 lineDir, Vector2 lineOrigin, Vector2 point)
    {
        Vector2 toPoint = point - lineOrigin;
        return lineDir.x * toPoint.y - lineDir.y * toPoint.x;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (TryGetSpriteRenderer(other.gameObject, out SpriteRenderer sr))
        {
            if (!_trackedObjects.ContainsKey(sr))
                _trackedObjects.Add(sr, sr.sortingOrder);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (TryGetSpriteRenderer(other.gameObject, out SpriteRenderer sr))
        {
            if (_trackedObjects.TryGetValue(sr, out int baseSortingOrder))
            {
                sr.sortingOrder = baseSortingOrder;
                _trackedObjects.Remove(sr);
            }
        }
    }

    private bool TryGetSpriteRenderer(GameObject obj, out SpriteRenderer sr)
    {
        if (obj.TryGetComponent(out sr)) return true;
        sr = obj.GetComponentInParent<SpriteRenderer>();
        if (sr != null) return true;
        sr = obj.GetComponentInChildren<SpriteRenderer>();
        return sr != null;
    }

    private void OnDrawGizmos()
    {
        if (_linePointA == null || _linePointB == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_linePointA.position, _linePointB.position);
    }
}