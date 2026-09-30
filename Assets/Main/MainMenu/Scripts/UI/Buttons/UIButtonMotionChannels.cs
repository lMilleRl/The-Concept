using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public sealed class UIButtonMotionChannels : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector2 _baseAnchoredPosition;
    private Vector3 _baseScale;
    private Vector2 _hoverOffset;
    private float _waveOffset;
    private float _hoverScale = 1f;
    private float _clickScale = 1f;
    private bool _effectsActive = true;
    private Vector3[] _buttonCorners;
    private Tween _entranceTween;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _baseAnchoredPosition = _rectTransform.anchoredPosition;
        _baseScale = _rectTransform.localScale;
        _buttonCorners = new Vector3[4];
    }

    public void SetHoverOffset(Vector2 value)
    {
        _hoverOffset = value;
    }

    public void SetWaveOffset(float value)
    {
        _waveOffset = value;
    }

    public void SetHoverScale(float value)
    {
        _hoverScale = value;
    }

    public void SetClickScale(float value)
    {
        _clickScale = value;
    }

    public void PlayEntrance(
        RectTransform viewport,
        Vector3 destination,
        bool fromRight,
        float delay,
        float duration,
        float overshoot,
        float edgePadding,
        Action onComplete)
    {
        _entranceTween?.Kill();
        _effectsActive = false;
        _hoverOffset = Vector2.zero;
        _waveOffset = 0f;
        _hoverScale = 1f;
        _clickScale = 1f;

        Transform parent = _rectTransform.parent;
        Vector3 destinationLocal = viewport.InverseTransformPoint(destination);
        _rectTransform.GetWorldCorners(_buttonCorners);

        float halfWidth = 0f;
        for (int i = 0; i < _buttonCorners.Length; i++)
        {
            float cornerX = viewport.InverseTransformPoint(_buttonCorners[i]).x;
            halfWidth = Mathf.Max(halfWidth, Mathf.Abs(cornerX - destinationLocal.x));
        }

        float startX = fromRight
            ? viewport.rect.xMax + halfWidth + edgePadding
            : viewport.rect.xMin - halfWidth - edgePadding;
        Vector3 startWorld = viewport.TransformPoint(
            new Vector3(startX, destinationLocal.y, destinationLocal.z));
        Vector3 startLocal = parent.InverseTransformPoint(startWorld);
        Vector3 destinationParentLocal = parent.InverseTransformPoint(destination);
        Vector2 startPosition = new Vector2(startLocal.x, startLocal.y);
        Vector2 destinationPosition = new Vector2(destinationParentLocal.x, destinationParentLocal.y);
        _baseAnchoredPosition = startPosition;

        float progress = 0f;
        _entranceTween = DOTween.To(
                () => progress,
                value =>
                {
                    progress = value;
                    _baseAnchoredPosition = Vector2.LerpUnclamped(startPosition, destinationPosition, value);
                },
                1f,
                duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack, overshoot)
            .OnComplete(() =>
            {
                _baseAnchoredPosition = destinationPosition;
                _effectsActive = true;
                onComplete?.Invoke();
            });
    }

    private void LateUpdate()
    {
        Vector2 offset = _effectsActive
            ? _hoverOffset + new Vector2(0f, _waveOffset)
            : Vector2.zero;
        _rectTransform.anchoredPosition = _baseAnchoredPosition + offset;
        _rectTransform.localScale = _baseScale * (_effectsActive ? _hoverScale * _clickScale : 1f);
    }

    private void OnDisable()
    {
        _entranceTween?.Kill();
    }
}
