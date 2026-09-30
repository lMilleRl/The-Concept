using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public sealed class UIButtonAlphaEffect : UIButtonHoverEffectBase
{
    [Range(0f, 1f)] [SerializeField] private float _idleAlpha = 0.65f;
    [Min(0f)] [SerializeField] private float _duration = 0.12f;

    private CanvasGroup _canvasGroup;
    private float _progress;
    private Tween _tween;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = _idleAlpha;
    }

    protected override void OnActivationZoneEntered()
    {
        Animate(1f);
    }

    protected override void OnActivationZoneExited()
    {
        Animate(0f);
    }

    protected override void OnEffectDisabled()
    {
        _tween?.Kill();
        _progress = 0f;
        _canvasGroup.alpha = _idleAlpha;
    }

    private void Update()
    {
        _canvasGroup.alpha = Mathf.Lerp(_idleAlpha, 1f, _progress);
    }

    private void Animate(float target)
    {
        _tween?.Kill();
        _tween = DOTween.To(() => _progress, value => _progress = value, target, _duration)
            .SetEase(Ease.OutCubic);
    }
}
