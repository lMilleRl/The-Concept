using DG.Tweening;
using UnityEngine;

public class GhostMemoryProximityDissolver : MonoBehaviour
{
    [Header("Proximity")]
    [Tooltip("Трансформ игрока, от которого измеряется расстояние")]
    [SerializeField] private Transform _playerTransform;
    [Tooltip("Первый центр зоны исчезновения")]
    [SerializeField] private Transform _referencePointA;
    [Tooltip("Второй центр зоны исчезновения")]
    [SerializeField] private Transform _referencePointB;
    [Tooltip("Общий радиус для обеих точек, в Unity units")]
    [SerializeField, Min(0f)] private float _dissolveRadius = 1f;

    [Header("Dissolve")]
    [Tooltip("SpriteRenderer обеих памятей; они растворяются вместе")]
    [SerializeField] private SpriteRenderer[] _memorySpriteRenderers;
    [Tooltip("Длительность растворения в секундах")]
    [SerializeField, Min(0f)] private float _dissolveDurationInSec = 0.5f;

    private Sequence _dissolveSequence;
    private bool _isDissolving;
    private bool _hasDissolved;

    private void Update()
    {
        if (_hasDissolved || _isDissolving || _playerTransform == null)
            return;

        var playerPosition = (Vector2)_playerTransform.position;
        var radius = Mathf.Max(0f, _dissolveRadius);
        var radiusSquared = radius * radius;

        if (IsWithinRadius(_referencePointA, playerPosition, radiusSquared) ||
            IsWithinRadius(_referencePointB, playerPosition, radiusSquared))
        {
            BeginDissolve();
        }
    }

    private bool IsWithinRadius(Transform referencePoint, Vector2 playerPosition, float radiusSquared)
    {
        if (referencePoint == null)
            return false;

        var offset = (Vector2)referencePoint.position - playerPosition;
        return offset.sqrMagnitude <= radiusSquared;
    }

    private void BeginDissolve()
    {
        if (_memorySpriteRenderers == null || _memorySpriteRenderers.Length == 0)
            return;

        bool hasSpriteRenderer = false;
        foreach (var spriteRenderer in _memorySpriteRenderers)
        {
            if (spriteRenderer == null)
                continue;

            hasSpriteRenderer = true;
            break;
        }

        if (!hasSpriteRenderer)
        {
            _hasDissolved = true;
            return;
        }

        _isDissolving = true;
        var duration = Mathf.Max(0f, _dissolveDurationInSec);
        if (duration == 0f)
        {
            CompleteDissolve();
            return;
        }

        _dissolveSequence = DOTween.Sequence();
        foreach (var spriteRenderer in _memorySpriteRenderers)
        {
            if (spriteRenderer != null)
                _dissolveSequence.Join(spriteRenderer.DOFade(0f, duration).SetEase(Ease.Linear));
        }

        _dissolveSequence.OnComplete(CompleteDissolve);
    }

    private void CompleteDissolve()
    {
        _dissolveSequence = null;
        _isDissolving = false;
        _hasDissolved = true;

        foreach (var spriteRenderer in _memorySpriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
        }
    }

    private void OnDisable()
    {
        _dissolveSequence?.Kill();
        _dissolveSequence = null;
        _isDissolving = false;
    }
}
