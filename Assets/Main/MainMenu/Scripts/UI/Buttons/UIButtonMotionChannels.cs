using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public sealed class UIButtonMotionChannels : MonoBehaviour
{
    private const float SmoothForce = 5f;

    private RectTransform _rectTransform;
    private Vector2 _baseAnchoredPosition;
    private Vector3 _baseScale;
    private Vector2 _hoverOffset;
    private Vector2 _appliedOffset;
    private float _waveOffset;
    private float _hoverScale = 1f;
    private float _clickScale = 1f;
    private bool _effectsActive = true;
    private Vector3[] _buttonCorners;
    private Tween _entranceTween;

    public event Action<float> EntranceStarted;
    public bool EffectsActive => _effectsActive;

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
        ButtonEntranceSide direction,
        float delay,
        float duration,
        float overshoot,
        float edgePadding,
        Action onComplete)
    {
        _entranceTween?.Kill();
        ResetMotionChannels();
        Vector2 startPosition = GetEntranceStartPosition(viewport, destination, direction, edgePadding);
        Vector2 destinationPosition = GetParentLocalPosition(destination);

        _baseAnchoredPosition = startPosition;
        AnimateEntrance(startPosition, destinationPosition, delay, duration, overshoot, onComplete);
    }

    private void LateUpdate()
    {
        SmoothMotionOffset(Time.deltaTime);
        ApplyTransform();
    }

    private void ResetMotionChannels()
    {
        _effectsActive = false;
        _hoverOffset = Vector2.zero;
        _waveOffset = 0f;
        _hoverScale = 1f;
        _clickScale = 1f;
    }

    private Vector2 GetEntranceStartPosition(
        RectTransform viewport,
        Vector3 destination,
        ButtonEntranceSide direction,
        float edgePadding)
    {
        Vector3 destinationLocal = viewport.InverseTransformPoint(destination);
        Vector2 halfSize = GetButtonHalfSize(viewport, destinationLocal.x, destinationLocal.y);

        return direction switch
        {
            ButtonEntranceSide.Left => GetStartPosition(viewport, destinationLocal, halfSize, Vector2.left, edgePadding),
            ButtonEntranceSide.Right => GetStartPosition(viewport, destinationLocal, halfSize, Vector2.right, edgePadding),
            ButtonEntranceSide.Top => GetStartPosition(viewport, destinationLocal, halfSize, Vector2.up, edgePadding),
            ButtonEntranceSide.Bottom => GetStartPosition(viewport, destinationLocal, halfSize, Vector2.down, edgePadding)
        };
    }

    private Vector2 GetButtonHalfSize(RectTransform viewport, float centerX, float centerY)
    {
        _rectTransform.GetWorldCorners(_buttonCorners);
        float halfWidth = 0f;
        float halfHeight = 0f;

        for (int i = 0; i < _buttonCorners.Length; i++)
        {
            Vector3 corner = viewport.InverseTransformPoint(_buttonCorners[i]);
            halfWidth = Mathf.Max(halfWidth, Mathf.Abs(corner.x - centerX));
            halfHeight = Mathf.Max(halfHeight, Mathf.Abs(corner.y - centerY));
        }

        return new Vector2(halfWidth, halfHeight);
    }

    private Vector2 GetStartPosition(
        RectTransform viewport,
        Vector3 destinationLocal,
        Vector2 buttonHalfSize,
        Vector2 direction,
        float edgePadding)
    {
        Vector2 viewportCenter = viewport.rect.center;
        Vector2 viewportHalfSize = viewport.rect.size * 0.5f;
        Vector2 startAtEdge = viewportCenter + Vector2.Scale(
            direction,
            viewportHalfSize + buttonHalfSize + Vector2.one * edgePadding);
        Vector2 startLocal = new Vector2(
            Mathf.Lerp(destinationLocal.x, startAtEdge.x, Mathf.Abs(direction.x)),
            Mathf.Lerp(destinationLocal.y, startAtEdge.y, Mathf.Abs(direction.y)));

        Vector3 startWorld = viewport.TransformPoint(
            new Vector3(startLocal.x, startLocal.y, destinationLocal.z));
        return GetParentLocalPosition(startWorld);
    }

    private Vector2 GetParentLocalPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = _rectTransform.parent.InverseTransformPoint(worldPosition);
        return new Vector2(localPosition.x, localPosition.y);
    }

    private void AnimateEntrance(
        Vector2 startPosition,
        Vector2 destinationPosition,
        float delay,
        float duration,
        float overshoot,
        Action onComplete)
    {
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
            .OnStart(() => EntranceStarted?.Invoke(duration))
            .OnComplete(() =>
            {
                _baseAnchoredPosition = destinationPosition;
                _effectsActive = true;
                onComplete?.Invoke();
            });
    }

    private void SmoothMotionOffset(float deltaTime)
    {
        Vector2 targetOffset = GetTargetOffset();
        _appliedOffset = SmoothLerp(_appliedOffset, targetOffset, deltaTime);
    }

    private void ApplyTransform()
    {
        _rectTransform.anchoredPosition = _baseAnchoredPosition + _appliedOffset;
        _rectTransform.localScale = _baseScale * (_effectsActive ? _hoverScale * _clickScale : 1f);
    }

    private Vector2 GetTargetOffset()
    {
        return _effectsActive
            ? _hoverOffset + new Vector2(0f, _waveOffset)
            : Vector2.zero;
            
    }

    private static Vector2 SmoothLerp(Vector2 current, Vector2 target, float deltaTime)
    {
        float t = SmoothT(deltaTime);
        return Vector2.Lerp(current, target, t);
    }

    private static float SmoothT(float deltaTime)
    {
        return 1f - Mathf.Exp(-SmoothForce * deltaTime);
    }

    private void OnDisable()
    {
        _entranceTween?.Kill();
    }
}
