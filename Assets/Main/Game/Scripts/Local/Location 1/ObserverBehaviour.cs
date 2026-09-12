using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class ObserverBehaviour : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 _moveOffset = new Vector3(0f, -3f, 0f);
    [FormerlySerializedAs("_escapeDuration")]
    [SerializeField, Min(0f)] private float _moveDuration = 0.4f;
    [FormerlySerializedAs("_escapeEase")]
    [SerializeField] private Ease _moveEase = Ease.InQuad;

    [Header("Sprite Fade")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField, Range(0f, 1f)] private float _targetAlpha = 0f;
    [SerializeField, Min(0f)] private float _fadeDuration = 0.4f;
    [SerializeField] private Ease _fadeEase = Ease.InQuad;

    [Header("Escape")]
    [SerializeField] private bool _deactivateAfterEscape = true;

    private Tween _moveTween;
    private Tween _fadeTween;
    private Tween _escapeTimer;
    private bool _isEscaping;

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    public void Move()
    {
        _moveTween?.Kill();
        _moveTween = transform.DOMove(transform.position + _moveOffset, _moveDuration)
            .SetEase(_moveEase);
    }

    public void FadeSprite()
    {
        if (_spriteRenderer == null)
            return;

        _fadeTween?.Kill();
        _fadeTween = _spriteRenderer.DOFade(_targetAlpha, _fadeDuration)
            .SetEase(_fadeEase);
    }

    public void Escape()
    {
        if (_isEscaping) return;
        _isEscaping = true;

        Move();
        FadeSprite();

        float duration = Mathf.Max(_moveDuration, _spriteRenderer != null ? _fadeDuration : 0f);
        _escapeTimer = DOVirtual.DelayedCall(duration, CompleteEscape, false);
    }

    private void CompleteEscape()
    {
        _escapeTimer = null;

        if (_deactivateAfterEscape)
            gameObject.SetActive(false);

        _isEscaping = false;
    }

    private void OnDisable()
    {
        _moveTween?.Kill();
        _fadeTween?.Kill();
        _escapeTimer?.Kill();

        _moveTween = null;
        _fadeTween = null;
        _escapeTimer = null;
        _isEscaping = false;
    }
}
